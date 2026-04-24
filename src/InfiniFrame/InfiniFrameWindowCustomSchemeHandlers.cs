// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using System.Collections.Concurrent;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowCustomSchemeHandlers : IInfiniFrameWindowCustomSchemeHandlers {
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
            lock (Lock) {
                OrderedRegisteredMessageIds.Add(messageId);
            }

            return;
        }

        Handlers.AddOrUpdate(messageId, handler, updateValueFactory: (_, oldHandler) => oldHandler + handler);
    }

    public IEnumerable<(string, NetCustomSchemeDelegate)> GetRegisteredHandlers() {
        List<string> snapshot;
        lock (Lock) {
            snapshot = OrderedRegisteredMessageIds.ToList();
        }

        return snapshot
            .Where(id => Handlers.TryGetValue(id, out _))
            .Select(id => (id, Handlers[id]));
    }

    public bool TryGetHandler(string scheme, out NetCustomSchemeDelegate? netCustomSchemeDelegate) => Handlers.TryGetValue(scheme, out netCustomSchemeDelegate);

    internal InfiniFrameWindowCustomSchemeHandlersSnapshot ToSnapshot() {
        List<string> orderedSnapshot;
        lock (Lock) {
            orderedSnapshot = OrderedRegisteredMessageIds.ToList();
        }

        return new InfiniFrameWindowCustomSchemeHandlersSnapshot(
            orderedSnapshot.ToArray(),
            Handlers.ToArray());
    }

    internal static InfiniFrameWindowCustomSchemeHandlers FromSnapshot(InfiniFrameWindowCustomSchemeHandlersSnapshot snapshot) {
        var copy = new InfiniFrameWindowCustomSchemeHandlers();

        Dictionary<string, NetCustomSchemeDelegate> handlers = snapshot.Handlers.ToDictionary(keySelector: static item => item.Key, elementSelector: static item => item.Value, StringComparer.Ordinal);
        foreach (var entry in snapshot.OrderedSchemeNames
            .Distinct(StringComparer.Ordinal)
            .Select(key => new { Key = key, Found = handlers.TryGetValue(key, out NetCustomSchemeDelegate? handler), Handler = handler })
            .Where(item => item.Found)) {
            copy.Handlers.TryAdd(entry.Key, entry.Handler!);
            copy.OrderedRegisteredMessageIds.Add(entry.Key);
        }

        return copy;
    }

    public bool ContainsCustomSchemeHandler(string onlyFirst) => Handlers.ContainsKey(onlyFirst);
}
