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
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterCustomSchemeHandler(string messageId, NetCustomSchemeDelegate handler) {
        Handlers.AddOrUpdate(messageId, handler,  (_, oldHandler) => oldHandler + handler);
        OrderedRegisteredMessageIds.Add(messageId);
    }
    
    public IEnumerable<(string, NetCustomSchemeDelegate)> GetRegisteredHandlers() {
        return OrderedRegisteredMessageIds.Select(id => (id, Handlers[id]));
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