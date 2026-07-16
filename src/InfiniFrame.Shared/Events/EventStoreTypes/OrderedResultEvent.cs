// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record OrderedResultEvent<TPayload, TResult> {
    private ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>> _handlers = ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>>.Empty;
    public ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Add(item), handler);
    }

    public void Remove(Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Remove(item), handler);
    }

    public TResult?[] Invoke(IInfiniFrameWindow window, TPayload payload) {
        var results = new TResult?[_handlers.Length];
        for (int i = 0; i < _handlers.Length; i++) {
            Func<IInfiniFrameWindow, TPayload, TResult> handler = _handlers[i];
            
            try {
                results[i] = handler(window, payload);
            }
            catch (Exception ex) when (ex is not OperationCanceledException) {
                results[i] = default;
            }
        }

        return results.ToArray();
    }
}
