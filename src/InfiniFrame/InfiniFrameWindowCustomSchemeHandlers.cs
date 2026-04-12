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

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterCustomSchemeHandler(string messageId, NetCustomSchemeDelegate handler) {
        Handlers.AddOrUpdate(messageId, handler, (_, _) => handler);
    }
    
    internal static InfiniFrameWindowCustomSchemeHandlers CopyFrom(InfiniFrameWindowCustomSchemeHandlers source) {
        ArgumentNullException.ThrowIfNull(source);
        
        var copy = new InfiniFrameWindowCustomSchemeHandlers();
        
        foreach ((string key, NetCustomSchemeDelegate value) in source.Handlers) {
            copy.RegisterCustomSchemeHandler(key, value);
        }
        
        return copy;
    }

}