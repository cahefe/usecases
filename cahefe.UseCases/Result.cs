namespace cahefe.UseCases;

/// <summary>
/// Represents a list of results
/// </summary>
public class Results : List<Result> { }

/// <summary>
/// Represents a list of results with a typed response
/// </summary>
/// <typeparam name="TResp">Typed response</typeparam>
public class Results<TResp> : List<Result<TResp>> { }

/// <summary>
/// Represents the result of an excution of use cases
/// </summary>
/// <param name="Fails">List of fails stored during de execution of the use case</param>
public record Result(params Fail[] Fails)
{
    /// <summary>
    /// Indicates if the execution was successful
    /// </summary>
    public bool Success => !Fails?.Any(f => f.FailLevel != FailLevel.Warning) ?? true;
    /// <summary>
    /// Automatically creates a successful result
    /// </summary>
    /// <returns>A successful result of use case execution</returns>
    public static Result GoSuccess() => new();
    /// <summary>
    /// Automatically creates a failed result
    /// </summary>
    /// <param name="fails">List of fails capture during de execution of the use case</param>
    /// <returns>List of fails of an use case</returns>
    public static Result GoFails(params Fail[] fails) => new(fails);
}
/// <summary>
/// Represents the result of an excution of use cases with a typed response
/// </summary>
/// <typeparam name="TResp">Type of response</typeparam>
/// <param name="Response">Response of use case execution</param>
/// <param name="Fails">List of fails stored during de execution of the use case</param>
public record Result<TResp>(TResp? Response = default, params Fail[] Fails) : Result(Fails)
{
    /// <summary>
    /// Automatically creates a successful result
    /// </summary>
    /// <param name="response">Response content</param>
    /// <returns>A successful result of use case execution</returns>
    public static Result<TResp> GoSuccess(TResp response) => new(response);
    public static new Result<TResp> GoFails(params Fail[] fails) => new(default, fails);
}