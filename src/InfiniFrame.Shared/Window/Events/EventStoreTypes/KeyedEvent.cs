// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a keyed event that maps keys to handlers with a payload.
/// </summary>
/// <typeparam name="TKey">The type of the key used to identify handlers.</typeparam>
/// <typeparam name="TPayload">The type of the payload passed to handlers.</typeparam>
public sealed record KeyedEvent<TKey, TPayload> where TKey : notnull {
    private ConcurrentDictionary<TKey, Action<IInfiniFrameWindow, TPayload>> Handlers { get; } = [];
    /// <summary>
    ///     Gets a snapshot of the current handler registrations.
    /// </summary>
    public IEnumerable<KeyValuePair<TKey, Action<IInfiniFrameWindow, TPayload>>> Snapshot => Handlers.ToArray();
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
    public void Add(TKey key, Action<IInfiniFrameWindow, TPayload> handler) {
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
    ///     Attempts to invoke the handler for the specified key.
    /// </summary>
    /// <param name="key">The key identifying the handler to invoke.</param>
    /// <param name="window">The window instance to pass to the handler.</param>
    /// <param name="payload">The payload to pass to the handler.</param>
    /// <returns><c>true</c> if a handler was found and invoked successfully; otherwise, <c>false</c>.</returns>
    public bool TryInvoke(TKey key, IInfiniFrameWindow window, TPayload payload) {
        if (!Handlers.TryGetValue(key, out Action<IInfiniFrameWindow, TPayload>? handler)) return false;
        
        try {
            handler(window, payload);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException) {
            return false;
        }
    }
    
    /// <summary>
    ///     Determines whether the specified key has a registered handler.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(TKey key) => Handlers.ContainsKey(key);
}
