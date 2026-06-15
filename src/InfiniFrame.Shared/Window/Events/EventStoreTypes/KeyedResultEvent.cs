// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record KeyedResultEvent<TKey, TPayload, TResult> where TKey : notnull {
    public ConcurrentDictionary<TKey, Func<IInfiniFrameWindow, TPayload, TResult>> Handlers { get; } = [];
    public int Count => Handlers.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(TKey key, Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);
        Handlers.AddOrUpdate(key, handler, (_, _) => handler);
    }

    public void Remove(TKey key) {
        ArgumentNullException.ThrowIfNull(key);
        Handlers.TryRemove(key, out _);
    }
    
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
    
    public bool ContainsKey(TKey key) => Handlers.ContainsKey(key);
}
