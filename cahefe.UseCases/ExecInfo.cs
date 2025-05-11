namespace cahefe.UseCases;
/// <summary>
/// Information about the execution, including monitoring data and statistics.
/// </summary>
internal class ExecInfo
{
    /// <summary>
    /// Execution time (milliseconds).
    /// </summary>
    public long ExecTime { get; set; }
    /// <summary>
    /// Number of steps executed.
    /// </summary>
    public int ExecSteps { get; set; }
    /// <summary>
    /// Number of retries.
    /// </summary>
    public byte Retries { get; set; }
}
