// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record KeyedWindowEvent<TKey, TPayload> where TKey : notnull {
    private ConcurrentDictionary<TKey, Action<IInfiniFrameWindow, TPayload>> Handlers { get; } = [];
    public int Count => Handlers.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(TKey key, Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(handler);
        Handlers.AddOrUpdate(key, handler, (_, _) => handler);
    }

    public void Remove(TKey key) {
        ArgumentNullException.ThrowIfNull(key);
        Handlers.TryRemove(key, out _);
    }
    
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
    
    public bool ContainsKey(TKey key) => Handlers.ContainsKey(key);
}
