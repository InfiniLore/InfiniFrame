// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOrderedClosingEvent {
    private ImmutableArray<NetClosingDelegate> _handlers = ImmutableArray<NetClosingDelegate>.Empty;
    public ImmutableArray<NetClosingDelegate> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(NetClosingDelegate handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, static (current, item) => current.Add(item), handler);
    }

    public void Remove(NetClosingDelegate handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, static (current, item) => current.Remove(item), handler);
    }

    public bool? Invoke(IInfiniFrameWindow window) {
        bool? result = null;
        foreach (NetClosingDelegate handler in _handlers) {
            result = handler(window, null);
        }

        return result;
    }
}
