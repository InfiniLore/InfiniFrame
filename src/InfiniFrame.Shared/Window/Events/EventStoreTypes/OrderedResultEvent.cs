// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an ordered collection of handlers that return a result and are invoked in sequence.
/// </summary>
/// <typeparam name="TPayload">The type of the payload passed to handlers.</typeparam>
/// <typeparam name="TResult">The type of the result returned by each handler.</typeparam>
public sealed record OrderedResultEvent<TPayload, TResult> {
    private ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>> _handlers = ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>>.Empty;
    /// <summary>
    ///     Gets a snapshot of the current handler registrations.
    /// </summary>
    public ImmutableArray<Func<IInfiniFrameWindow, TPayload, TResult>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds a handler to the end of the invocation list.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void Add(Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Add(item), handler);
    }

    /// <summary>
    ///     Removes a handler from the invocation list.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    public void Remove(Func<IInfiniFrameWindow, TPayload, TResult> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Remove(item), handler);
    }

    /// <summary>
    ///     Invokes all registered handlers in order and collects their results.
    /// </summary>
    /// <param name="window">The window instance to pass to each handler.</param>
    /// <param name="payload">The payload to pass to each handler.</param>
    /// <returns>An array of results from each handler, with null values for handlers that threw non-fatal exceptions.</returns>
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
