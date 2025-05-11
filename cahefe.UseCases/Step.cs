namespace cahefe.UseCases;
/// <summary>
/// Represents a step in the pipeline.
/// </summary>
/// <param name="Phase">Phase of the step</param>
/// <param name="Name">Step's name</param>
/// <param name="StepType">Type of step</param>
/// <param name="UseCase">Type of use case</param>
/// <param name="Retries">Number of retries</param>
/// <param name="RetryInterval">Time between retries (in milliseconds</param>
/// <param name="Counters">Counters</param>
internal record Step
(
    Phase Phase,
    FlowType StepType,
    Type UseCase,
    string? Name = null,
    byte Retries = 0,
    int RetryInterval = 0
)
{
    public ExecInfo Counters { get; set; } = new();
}
