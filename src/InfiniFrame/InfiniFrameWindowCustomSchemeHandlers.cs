// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowCustomSchemeHandlers : IInfiniFrameWindowCustomSchemeHandlers{
    public bool IsEmpty => Handlers.IsEmpty;
    public int Length => Handlers.Count;
    private ConcurrentDictionary<string, NetCustomSchemeDelegate> Handlers { get; } = new();
    private List<string> OrderedRegisteredMessageIds { get; } = new();
    
    #if NET9_0_OR_GREATER
    private Lock Lock { get; } = new();
    #else     
    private object Lock { get; } = new();
    #endif
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterCustomSchemeHandler(string messageId, NetCustomSchemeDelegate handler) {
        if (Handlers.TryAdd(messageId, handler)) {
            lock (Lock) OrderedRegisteredMessageIds.Add(messageId);
            return;
        }

        Handlers.AddOrUpdate(messageId, handler, (_, oldHandler) => oldHandler + handler);
    }
    
    public IEnumerable<(string, NetCustomSchemeDelegate)> GetRegisteredHandlers() {
        List<string> snapshot;
        lock (Lock) snapshot = OrderedRegisteredMessageIds.ToList();

        foreach (string id in snapshot) {
            if (Handlers.TryGetValue(id, out NetCustomSchemeDelegate? handler)) {
                yield return (id, handler);
            }
        }
    }

    public bool TryGetHandler(string scheme, out NetCustomSchemeDelegate? netCustomSchemeDelegate) {
        return Handlers.TryGetValue(scheme, out netCustomSchemeDelegate);
    }

    internal static InfiniFrameWindowCustomSchemeHandlers CopyFrom(InfiniFrameWindowCustomSchemeHandlers source) {
        ArgumentNullException.ThrowIfNull(source);
        
        var copy = new InfiniFrameWindowCustomSchemeHandlers();
        
        foreach ((string key, NetCustomSchemeDelegate value) in source.GetRegisteredHandlers()) {
            copy.RegisterCustomSchemeHandler(key, value);
        }
        
        return copy;
    }

    public bool ContainsCustomSchemeHandler(string onlyFirst) {
        return Handlers.ContainsKey(onlyFirst);
    }
}
