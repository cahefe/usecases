namespace cahefe.UseCases;
public interface IUseCase
{
    Task<Result> Execute(CancellationToken cancellationToken = default);
}
public interface IUseCase<TReq, TResp>
{
    Task<Result<TResp>> Execute(TReq req, CancellationToken cancellationToken = default);
}
