// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OrderedEvent {
    private ImmutableArray<Action<IInfiniFrameWindow>> _handlers = ImmutableArray<Action<IInfiniFrameWindow>>.Empty;
    public ImmutableArray<Action<IInfiniFrameWindow>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(Action<IInfiniFrameWindow> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(
            ref _handlers,
            transformer: static (current, item) => current.Add(item),
            handler
        );
    }

    public void Remove(Action<IInfiniFrameWindow> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(
            ref _handlers,
            transformer: static (current, item) => current.Remove(item),
            handler
        );
    }

    public void Invoke(IInfiniFrameWindow window) {
        foreach (Action<IInfiniFrameWindow> handler in _handlers) {
            handler(window);
        }
    }
}

public class OrderedEvent<TPayload> {
    private ImmutableArray<Action<IInfiniFrameWindow, TPayload>> _handlers = ImmutableArray<Action<IInfiniFrameWindow, TPayload>>.Empty;
    public ImmutableArray<Action<IInfiniFrameWindow, TPayload>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Add(item), handler);
    }

    public void Remove(Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Remove(item), handler);
    }

    public void Invoke(IInfiniFrameWindow window, TPayload payload) {
        foreach (Action<IInfiniFrameWindow, TPayload> handler in _handlers) {
            handler(window, payload);
        }
    }
}