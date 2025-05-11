using System.Collections.Concurrent;

namespace cahefe.UseCases;
/// <summary>
/// Holds data that is passed between the pipeline stages and steps.
/// </summary>
/// <typeparam name="TReq">Typed request data</typeparam>
/// <typeparam name="TResp">Typed response data</typeparam>
public class Bag<TReq, TResp>
{
    /// <summary>
    /// Data staged for the use case.
    /// </summary>
    public required TReq Request { get; set; }
    public required TResp Response { get; set; }
    readonly ConcurrentDictionary<string, object> _data = new();

    public void Add(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        if (_data.ContainsKey(key))
            throw new ArgumentException($"Key '{key}' already exists.", nameof(key));

        _data.TryAdd(key, value);
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        if (_data.TryGetValue(key, out var objValue))
        {
            value = (T)objValue;
            return true;
        }

        value = default!;
        return false;
    }
}
