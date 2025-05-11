namespace cahefe.UseCases.Console;

public class ResponseInfo
{
    public required string ResponseNameValue { get; set; }
    public int ResponseValue { get; set; }
    public readonly DateTime ResponseTime = DateTime.Now;
}
