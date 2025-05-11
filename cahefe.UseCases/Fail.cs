namespace cahefe.UseCases;

/// <summary>
/// Stores a list of fails.
/// </summary>
public class FailList : List<Fail>{};

/// <summary>
/// Stores information about a failed operation.
/// </summary>
/// <param name="Code">Code identification for a fail</param>
/// <param name="Mnemonic">Mnemonic identification for a fail (when available)</param>
/// <param name="Info">Information about the fail</param>
/// <param name="FailLevel">Level of severity</param>
public record Fail(int Code, string Mnemonic, string Info, FailLevel FailLevel = FailLevel.Error)
{
    /// <summary>
    /// Creates a new fail instance.
    /// </summary>
    /// <param name="code">Code identification for a fail</param>
    /// <param name="mnemonic">Mnemonic identification for a fail (when available)</param>
    /// <param name="info">Information about the fail</param>
    /// <returns></returns>
    public static Fail Raise(int code, string mnemonic, string info, FailLevel failLevel = FailLevel.Error) => new(code, mnemonic, info, failLevel);
}
