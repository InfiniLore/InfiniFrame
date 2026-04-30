// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "ConvertToExtensionBlock"), SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class InfiniWindowEventsExtensions {
    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when its location changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window and new location.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterLocationChangedHandler<T>(this T builder, Action<IInfiniFrameWindow, Point> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowLocationChanged.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when its size changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window and new size.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterSizeChangedHandler<T>(this T builder, Action<IInfiniFrameWindow, Size> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowSizeChanged.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
    ///     in.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterFocusInHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowFocusIn.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is maximized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterMaximizedHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowMaximized.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is restored.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterRestoredHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowRestored.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
    ///     out.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterFocusOutHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowFocusOut.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is minimized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterMinimizedHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowMinimized.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it sends a message.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <remarks>
    ///     Messages should be sent from JavaScript via
    ///     <code>window.__infiniframe.host.postData({ id: "...", command: "Post", data: ..., version: 2 })</code>.
    /// </remarks>
    /// <param name="handler">Handler invoked with the window and message.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWebMessageReceivedHandler<T>(this T builder, Action<IInfiniFrameWindow, string> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WebMessageReceived.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it sends a message,
    ///     resolving required services from the configured service provider.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <remarks>
    ///     Messages should be sent from JavaScript via
    ///     <code>window.__infiniframe.host.postData({ id: "...", command: "Post", data: ..., version: 2 })</code>.
    /// </remarks>
    /// <param name="handler">Handler that receives the resolved service and web message data.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWebMessageReceivedHandler<T, TService>(this T builder, Action<TService, IInfiniFrameWindow, string> handler) where T : IHasInfiniFrameEvents where TService : notnull {
        ArgumentNullException.ThrowIfNull(handler);

        builder.Events.WebMessageReceived.Add((sender, message) => {
            IServiceProvider? provider = sender.ServiceProvider;
            if (provider is null) {
                throw new InvalidOperationException(
                    "Web message handlers with service injection were registered, but no IServiceProvider was supplied. " +
                    "Call Build(provider) or register non-DI handlers."
                );
            }

            var service = provider.GetRequiredService<TService>();
            handler(service, sender, message);
        });

        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder before the window is closed
    ///     through the native api calls.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWindowClosingRequestedHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowClosingRequested.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when the builder is about to
    ///     close.
    ///     Handler can return true to prevent the builder from closing.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">
    ///     <see cref="NetClosingDelegate" />
    /// </param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWindowClosingHandler<T>(this T builder, NetClosingDelegate handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowClosing.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks before the native builder is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWindowCreatingHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowCreating.Add(handler);
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks after the native builder is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
    /// </returns>
    /// <param name="handler">Handler invoked with the window.</param>
    /// <param name="builder">The builder to register the handler for.</param>
    public static T RegisterWindowCreatedHandler<T>(this T builder, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEvents {
        builder.Events.WindowCreated.Add(handler);
        return builder;
    }
}
