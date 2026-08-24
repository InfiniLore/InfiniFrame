// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a keyed event that maps keys to handlers returning a result.
/// </summary>
public sealed record KeyedResultEvent<TKey, TPayload, TResult>
    where TKey : notnull {

    private ImmutableDictionary<TKey, Func<IInfiniFrameWindow, TPayload, TResult>> _handlers =
        ImmutableDictionary<TKey, Func<IInfiniFrameWindow, TPayload, TResult>>.Empty;

    /// <summary>
    ///     Gets an immutable snapshot of the current handler registrations.
    /// </summary>
    public ImmutableDictionary<TKey, Func<IInfiniFrameWindow, TPayload, TResult>> Snapshot => _handlers;

    /// <summary>
    ///     Gets the number of registered handlers.
    /// </summary>
    public int Count => _handlers.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds or updates a handler for the specified key.
    /// </summary>
    public void Add(TKey key, Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);

        ImmutableInterlocked.AddOrUpdate(
            ref _handlers,
            key,
            handler,
            updateValueFactory: (_, _) => handler
        );
    }

    /// <summary>
    ///     Removes the handler associated with the specified key.
    /// </summary>
    public void Remove(TKey key) {
        ArgumentNullException.ThrowIfNull(key);

        ImmutableInterlocked.TryRemove(
            ref _handlers,
            key,
            out _);
    }

    /// <summary>
    ///     Attempts to invoke the handler for the specified key and retrieve a result.
    /// </summary>
    /// <remarks>Handler exceptions propagate to the caller.</remarks>
    public bool TryInvoke(
        TKey key,
        IInfiniFrameWindow window,
        TPayload payload,
        out TResult? result
    ) {

        result = default;

        if (!_handlers.TryGetValue(key, out Func<IInfiniFrameWindow, TPayload, TResult>? handler))
            return false;

        result = handler(window, payload);

        return true;
    }

    /// <summary>
    ///     Determines whether the specified key has a registered handler.
    /// </summary>
    public bool ContainsKey(TKey key) => _handlers.ContainsKey(key);
}
