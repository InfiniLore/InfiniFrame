// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an ordered collection of handlers that are invoked in sequence.
/// </summary>
public sealed record OrderedEvent {
    private ImmutableArray<Action<IInfiniFrameWindow>> _handlers = ImmutableArray<Action<IInfiniFrameWindow>>.Empty;
    /// <summary>
    ///     Gets a snapshot of the current handler registrations.
    /// </summary>
    public ImmutableArray<Action<IInfiniFrameWindow>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds a handler to the end of the invocation list.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void Add(Action<IInfiniFrameWindow> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(
            ref _handlers,
            transformer: static (current, item) => current.Add(item),
            handler
        );
    }

    /// <summary>
    ///     Removes a handler from the invocation list.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    public void Remove(Action<IInfiniFrameWindow> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(
            ref _handlers,
            transformer: static (current, item) => current.Remove(item),
            handler
        );
    }

    /// <summary>
    ///     Invokes all registered handlers in registration order.
    /// </summary>
    /// <param name="window">The window instance to pass to each handler.</param>
    /// <remarks>Handler exceptions are propagated to the caller.</remarks>
    public void Invoke(IInfiniFrameWindow window) {
        foreach (Action<IInfiniFrameWindow> handler in _handlers) {
            handler(window);
        }
    }
}

/// <summary>
///     Represents an ordered collection of handlers with a payload that are invoked in sequence.
/// </summary>
/// <typeparam name="TPayload">The type of the payload passed to handlers.</typeparam>
public sealed record OrderedEvent<TPayload> {
    private ImmutableArray<Action<IInfiniFrameWindow, TPayload>> _handlers = ImmutableArray<Action<IInfiniFrameWindow, TPayload>>.Empty;
    /// <summary>
    ///     Gets a snapshot of the current handler registrations.
    /// </summary>
    public ImmutableArray<Action<IInfiniFrameWindow, TPayload>> Snapshot => _handlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Adds a handler to the end of the invocation list.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void Add(Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Add(item), handler);
    }

    /// <summary>
    ///     Adds a handler that resolves a service from the window's service provider before invocation.
    /// </summary>
    /// <param name="handler">The handler to add, receiving the resolved service.</param>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    public void AddWithServiceResolving<TService>(Action<IInfiniFrameWindow, TPayload, TService> handler) where TService : notnull {
        ArgumentNullException.ThrowIfNull(handler);

        ImmutableInterlocked.Update(ref _handlers, transformer: (current, item) => current.Add(item), ActionCallback);
        return;

        void ActionCallback(IInfiniFrameWindow window, TPayload payload) {
            IServiceProvider? provider = window.ServiceProvider;
            if (provider is null) throw new InvalidOperationException("Service provider is null, cannot resolve service.");

            var service = provider.GetRequiredService<TService>();
            handler(window, payload, service);
        }
    }

    /// <summary>
    ///     Removes a handler from the invocation list.
    /// </summary>
    /// <param name="handler">The handler to remove.</param>
    public void Remove(Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Remove(item), handler);
    }

    /// <summary>
    ///     Invokes all registered handlers in registration order with the specified payload.
    /// </summary>
    /// <param name="window">The window instance to pass to each handler.</param>
    /// <param name="payload">The payload to pass to each handler.</param>
    /// <remarks>Handler exceptions are propagated to the caller.</remarks>
    public void Invoke(IInfiniFrameWindow window, TPayload payload) {
        foreach (Action<IInfiniFrameWindow, TPayload> handler in _handlers) {
            handler(window, payload);
        }
    }
}
