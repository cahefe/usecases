namespace cahefe.UseCases.Console;
/// <summary>
/// Represents the data received by a request and should be shared between use cases.
/// </summary>
public class RequestInfo
{
    public required string Name { get; set; }
    public int Value { get; set; }
}
