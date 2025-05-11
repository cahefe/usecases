using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace cahefe.UseCases;
/// <summary>
/// Set of extension methods for the pipeline.
/// </summary>
public static class PipelineExtension
{
    /// <summary>
    /// Appends a phase to the pipeline.
    /// </summary>
    /// <typeparam name="TReq">Type of requisition</typeparam>
    /// <typeparam name="TResp">Type of response</typeparam>
    /// <param name="pipeline">Pipeline</param>
    /// <param name="name">Phase name</param>
    /// <returns>Updated pipeline</returns>
    /// <exception cref="ArgumentException">Fails to informe arguments</exception>
    public static Pipeline<TReq, TResp> AppendPhase<TReq, TResp>(this Pipeline<TReq, TResp> pipeline, [Required] string name)
    {

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"The phase name is required.");

        // Check if there is a phase with the same name
        if (pipeline._phases.Any(p => p.Name.Equals(name)))
            throw new ArgumentException($"The phase name ({name}) is already defined in the pipeline.");

        // Add new phase to the pipeline
        pipeline._phases.Add(new Phase(Order: (byte)pipeline._phases.Count(), Name: name, Steps: []));
        return pipeline;
    }

    /// <summary>
    /// Appends a step to the last phase defined in the pipeline.
    /// </summary>
    /// <typeparam name="TReq">Type of requisition</typeparam>
    /// <typeparam name="TResp">Type of response</typeparam>
    /// <param name="pipeline">Pipeline</param>
    /// <param name="useCase">Use case implementation</param>
    /// <param name="name">Step name</param>
    /// <param name="retries">Number of retries</param>
    /// <param name="retryInterval">Time between retries (in milliseconds)</param>
    /// <returns>Updated pipeline</returns>
    /// <exception cref="ArgumentException">Fails to informe arguments</exception>
    public static Pipeline<TReq, TResp> AppendStep<TReq, TResp, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TUseCase>(this Pipeline<TReq, TResp> pipeline, string? name = null, byte retries = 0, int retryInterval = 0) where TUseCase : class, IUseCase
    {
        //  Check retries and retry interval values have valid values
        if (retries < 0)
            throw new ArgumentException($"The number of retries must be greater than or equal to 0.");
        if (retryInterval < 0)
            throw new ArgumentException($"The retry interval must be greater than or equal to 0.");
        // Check if there is at least one phase defined
        if (!pipeline._phases.Any())
            throw new ArgumentException($"The pipeline has no phases defined.");

        var phase = pipeline._phases[pipeline._phases.Count - 1]; // Obtain the last phase

        //  Check when a names is passed if any step in the phase has the same name...
        if (!string.IsNullOrWhiteSpace(name) && phase.Steps.Any(s => s.Name is not null && s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            throw new ArgumentException($"The step name ({name}) is already used in this phase.");

        var useCase = typeof(TUseCase);

        //  Check if there is a step with the same use case in the phase
        if (phase.Steps.Any(s => s.UseCase == useCase))
            throw new ArgumentException($"The step pair ({phase.Name}/{nameof(useCase)}) is already defined in the pipeline.");

        //  Add the step to the phase
        phase.Steps.Add(new(Phase: phase, StepType: FlowType.Primary, UseCase: useCase, Name: name, Retries: retries, RetryInterval: retryInterval));

        return pipeline;
    }

    public static void Stats<TReq, TResp>(this Pipeline<TReq, TResp> pipeline)
    {
        //  Get the total steps executed
        var totalStepsExecuted = pipeline._phases.Sum(p => p.Counters.ExecSteps);
        //  Get the total retries executed
        var totalRetries = pipeline._phases.SelectMany(p => p.Steps).Sum(s => s.Counters.Retries);
        //  Get the total time of execution
        var totalTime = pipeline._phases.Sum(p => p.Counters.ExecTime);
        //  Get the total time of execution
        var totalExecTime = pipeline._phases.SelectMany(p => p.Steps).Sum(s => s.Counters.ExecTime);
        //  Get the performance of the pipeline (in percentage)
        var pipelinePerformance = totalTime == 0 ? 0.0 : totalExecTime / (double)totalTime;
        Console.WriteLine("Pipeline statistics:");
        Console.WriteLine($"Exec Time: Pipeline (Steps) {totalTime} ms ({totalExecTime} ms). Performance: {pipelinePerformance:P2}");
        Console.WriteLine($"Steps - Executed (Retries): {totalStepsExecuted} ({totalRetries})");
    }
}
