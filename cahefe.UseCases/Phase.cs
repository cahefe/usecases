namespace cahefe.UseCases;

/// <summary>
/// Represents a phase in the pipeline.
/// </summary>
/// <param name="Order">Order of execution</param>
/// <param name="Name">Phase's name</param>
/// <param name="Steps">List of steps</param>
/// <param name="Retries">Number of retries (for any step)</param>
/// <param name="RetryInterval">Time between retries (in milliseconds</param>
/// <param name="Counters">Counters</param>
internal record Phase
(
    byte Order,
    string Name,
    IList<Step> Steps,
    byte Retries = 0,
    int RetryInterval = 0
)
{
    public ExecInfo Counters { get; set; } = new();
}