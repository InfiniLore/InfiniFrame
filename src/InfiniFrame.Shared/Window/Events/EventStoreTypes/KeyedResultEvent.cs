// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a keyed event that maps keys to handlers returning a result.
/// </summary>
/// <typeparam name="TKey">The type of the key used to identify handlers.</typeparam>
/// <typeparam name="TPayload">The type of the payload passed to handlers.</typeparam>
/// <typeparam name="TResult">The type of the result returned by handlers.</typeparam>
public sealed record KeyedResultEvent<TKey, TPayload, TResult> where TKey : notnull {
    /// <summary>
    ///     Gets the dictionary of registered handlers.
    /// </summary>
    public ConcurrentDictionary<TKey, Func<IInfiniFrameWindow, TPayload, TResult>> Handlers { get; } = [];
    /// <summary>
    ///     Gets the number of registered handlers.
    /// </summary>
    public int Count => Handlers.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds or updates a handler for the specified key.
    /// </summary>
    /// <param name="key">The key to associate the handler with.</param>
    /// <param name="handler">The handler to invoke.</param>
    public void Add(TKey key, Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);
        Handlers.AddOrUpdate(key, handler, (_, _) => handler);
    }

    /// <summary>
    ///     Removes the handler associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the handler to remove.</param>
    public void Remove(TKey key) {
        ArgumentNullException.ThrowIfNull(key);
        Handlers.TryRemove(key, out _);
    }
    
    /// <summary>
    ///     Attempts to invoke the handler for the specified key and retrieve a result.
    /// </summary>
    /// <param name="key">The key identifying the handler to invoke.</param>
    /// <param name="window">The window instance to pass to the handler.</param>
    /// <param name="payload">The payload to pass to the handler.</param>
    /// <param name="result">The result from the handler, if invocation succeeded.</param>
    /// <returns><c>true</c> if a handler was found and returned a non-null result; otherwise, <c>false</c>.</returns>
    public bool TryInvoke(TKey key, IInfiniFrameWindow window, TPayload payload, [NotNullWhen(true)] out TResult? result) {
        result = default;
        if (!Handlers.TryGetValue(key, out Func<IInfiniFrameWindow, TPayload, TResult>? handler)) return false;
        
        try {
            result = handler(window, payload);
        }
        catch (Exception ex) when (ex is not OperationCanceledException) {
            result = default;
            return false;
        }
        
        return result is not null;
    }
    
    /// <summary>
    ///     Determines whether the specified key has a registered handler.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(TKey key) => Handlers.ContainsKey(key);
}
