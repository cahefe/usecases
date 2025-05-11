using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cahefe.UseCases;

public class Pipeline<TReq, TResp>(IServiceProvider serviceProvider, Bag<TReq, TResp> bag, ILogger<Pipeline<TReq, TResp>> logger) : IUseCase<TReq, TResp>
{
    internal IList<Phase> _phases = [];
    private FlowType _currentFlowType = FlowType.Primary;
    public async Task<Result<TResp>> Execute(TReq req, CancellationToken cancellationToken = default)
    {
        if (!HasPhases(out var phases))
            return Result<TResp>.GoFails(Fail.Raise(-1, "PIPELINE_HAS_NO_PHASES_DEFINED", "There are no phases defined in the pipeline.", FailLevel.Warning));

        //  Link request to the bag...
        if (req is not null)
            bag.Request = req;

        logger.LogTrace("Flow type: {FlowType}", _currentFlowType);

        Results results = [];
        var totalPhases = phases.Count();
        var phaseCount = 0;

        foreach (var phase in phases)
        {
            logger.LogTrace("Phase {Count}/{Total} {Name}...", ++phaseCount, totalPhases, phase.Name);

            var phaseResult = await ExecutePhase(phase, cancellationToken);
            results.AddRange(phaseResult);
            if (phaseResult.Any(r => !r.Success))
                return Result<TResp>.GoFails([.. phaseResult.SelectMany(r => r.Fails)]);
        }
        return Result<TResp>.GoSuccess(bag.Response);
    }

    /// <summary>
    /// Check if there are any phases in the pipeline that match the current flow type.
    /// </summary>
    /// <param name="phases"></param>
    /// <returns></returns>
    private bool HasPhases(out IEnumerable<Phase> phases)
    {
        phases = _phases.Where(p => p.Steps.Any(s => s.StepType.Equals(_currentFlowType)));
        return phases.Any();
    }

    private async Task<IEnumerable<Result>> ExecutePhase(Phase phase, CancellationToken cancellationToken)
    {
        var phaseStopWatch = Stopwatch.StartNew();
        var tasks = CreateStageTasks(phase, cancellationToken);
        await Task.WhenAll(tasks);
        phaseStopWatch.Stop();

        phase.Counters.ExecTime = phaseStopWatch.ElapsedMilliseconds;
        var results = tasks.Select(t => t.Result);

        return results.Any(r => !r.Success)
            ? results.ToArray()
            : [Result.GoSuccess()];
    }

    private IList<Task<Result>> CreateStageTasks(Phase phase, CancellationToken cancellationToken)
    {
        //  Set of steps in the phase that are of the current flow type
        var steps = phase.Steps.Where(s => s.StepType.Equals(_currentFlowType));
        var totalSteps = steps.Count();
        var stepCount = 0;
        var tasks = new List<Task<Result>>();

        foreach (var step in steps)
        {
            logger.LogTrace("Step {Count}/{Total}: {Name} (use case: {UseCase})...", ++stepCount, totalSteps, step.Name ?? "No Name", step.UseCase.Name);
            tasks.Add(ExecuteStep(step, cancellationToken));
        }
        return tasks;
    }

    private Task<Result> ExecuteStep(Step step, CancellationToken cancellationToken) => Task.Run(async () =>
    {
        var stepStopWatch = Stopwatch.StartNew();
        try
        {
            return await ExecuteWithRetries(step, cancellationToken);
        }
        finally
        {
            stepStopWatch.Stop();
            step.Counters.ExecTime += stepStopWatch.ElapsedMilliseconds;
            step.Phase.Counters.ExecSteps++;
        }
    }, cancellationToken);

    private async Task<Result> ExecuteWithRetries(Step step, CancellationToken cancellationToken)
    {
        var useCase = (IUseCase)serviceProvider.GetRequiredService(step.UseCase);
        var tries = 0;
        var maxTries = 1 + (step.Retries > 0 ? step.Retries : step.Phase.Retries);

        while (true)
        {
            try
            {
                return await useCase.Execute(cancellationToken);
            }
            catch (Exception ex)
            {
                ++tries;
                logger.LogError(ex, "Usecase/Try {UseCase}/{Try}: Exception ({Exception})\nMessage: {Message}", step.UseCase.Name, tries, ex.GetType().Name, ex.Message);
                if (tries >= maxTries)
                {
                    _currentFlowType = FlowType.Rollback;
                    return Result.GoFails(Fail.Raise(-1, "PIPELINE_ERROR_EXECUTING_USE_CASE", $"Error executing use case {step.UseCase.Name}.", FailLevel.Error));
                }
                step.Counters.Retries++;
                var interval = step.RetryInterval > 0 ? step.RetryInterval : step.Phase.RetryInterval;
                if (interval > 0) await Task.Delay(interval, cancellationToken);
            }
        }
    }
}
