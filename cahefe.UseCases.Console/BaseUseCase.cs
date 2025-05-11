using Microsoft.Extensions.Logging;

namespace cahefe.UseCases.Console;
public abstract class BaseUseCase(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : IUseCase
{
    protected readonly Bag<RequestInfo, ResponseInfo> stage = stage;
    protected readonly ILogger<BaseUseCase> logger = logger;
    public async virtual Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        int delay = Random.Shared.Next(300, 1500);
        await Task.Delay(delay, cancellationToken);
        var userInfo = stage.Request;
        userInfo.Value += delay;
        userInfo.Name += " " + GetType().Name;
        logger.LogInformation("{type} ({delay}): \"{name}\" | {value}", GetType().Name, delay, userInfo.Name, userInfo.Value);
        return Result.GoSuccess();
    }
}
