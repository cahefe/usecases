using Microsoft.Extensions.Logging;

namespace cahefe.UseCases.Console;
public class UseCasePreparation(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger)
{
    public async override Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        await base.Execute(cancellationToken);
        stage.Request.Value += 1;
        return Result.GoSuccess();
    }
}
public class UseCasePhase1(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class UseCasePhase21(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class UseCasePhase22(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class UseCasePhase23(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class UseCasePhase31(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class UseCasePhase32(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger) { }
public class ResponseUseCase(Bag<RequestInfo, ResponseInfo> stage, ILogger<BaseUseCase> logger) : BaseUseCase(stage, logger)
{
    public override Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var userInfo = stage.Request;
        logger.LogInformation("Response UseCase!");
        stage.Response = new ResponseInfo
        {
            ResponseNameValue = userInfo.Name + ": " + userInfo.Value,
            ResponseValue = Random.Shared.Next(100, 1000)
        };
        return Task.FromResult(Result.GoSuccess());
    }
}
