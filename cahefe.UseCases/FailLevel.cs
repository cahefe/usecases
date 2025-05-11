namespace cahefe.UseCases;

/// <summary>
/// Represents the severity of a fail in an operation
/// </summary>
public enum FailLevel : byte
{
    /// <summary>
    /// Represents a warning level fail.
    /// </summary>
    Warning,
    /// <summary>
    /// Represents an error level fail.
    /// </summary>
    Error,
    /// <summary>
    /// Represents a critical level fail.
    /// </summary>
    Critical
}
