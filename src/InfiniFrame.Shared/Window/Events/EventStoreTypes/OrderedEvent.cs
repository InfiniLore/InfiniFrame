// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record OrderedEvent {
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

public sealed record OrderedEvent<TPayload> {
    private ImmutableArray<Action<IInfiniFrameWindow, TPayload>> _handlers = ImmutableArray<Action<IInfiniFrameWindow, TPayload>>.Empty;
    public ImmutableArray<Action<IInfiniFrameWindow, TPayload>> Snapshot => _handlers;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Add(Action<IInfiniFrameWindow, TPayload> handler) {
        ArgumentNullException.ThrowIfNull(handler);
        ImmutableInterlocked.Update(ref _handlers, transformer: static (current, item) => current.Add(item), handler);
    }

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