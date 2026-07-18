// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging.Abstractions;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods for registering event handlers on objects that have an events store.
/// </summary>
public static class HasInfiniFrameEventsStoreExtensions {
    /// <summary>
    ///     Registers a handler that is invoked when the window location changes.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window and new location.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterLocationChangedHandler<T>(this T obj, Action<IInfiniFrameWindow, Point> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowLocationChanged.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window size changes.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window and new size.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterSizeChangedHandler<T>(this T obj, Action<IInfiniFrameWindow, Size> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowSizeChanged.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window receives focus.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterFocusInHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowFocusIn.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window is maximized.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterMaximizedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowMaximized.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window is restored.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterRestoredHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowRestored.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window loses focus.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterFocusOutHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowFocusOut.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window is minimized.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterMinimizedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowMinimized.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler for web messages received from the browser control.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window and message string.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessageReceivedHandler<T>(this T obj, Action<IInfiniFrameWindow, string> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessageReceived.Add((window, payload) => handler(window, payload.Message));
        return obj;
    }

    /// <summary>
    ///     Registers a handler for web messages that resolves a service before invocation.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window, message, and resolved service.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessageReceivedHandler<T, TService>(this T obj, Action<IInfiniFrameWindow, string, TService> handler) where TService : notnull where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessageReceived.AddWithServiceResolving<TService>((window, payload, service) => handler(window, payload.Message, service));
        return obj;
    }

    /// <summary>
    ///     Registers a handler for web messages that includes the message origin.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window, message, and origin.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessageReceivedHandler<T>(this T obj, Action<IInfiniFrameWindow, string, string?> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessageReceived.Add((window, payload) => handler(window, payload.Message, payload.Origin));
        return obj;
    }

    /// <summary>
    ///     Registers a handler for web messages that includes the origin and resolves a service before invocation.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window, message, origin, and resolved service.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <typeparam name="TService">The type of the service to resolve.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessageReceivedHandler<T, TService>(this T obj, Action<IInfiniFrameWindow, string, string?, TService> handler) where TService : notnull where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessageReceived.AddWithServiceResolving<TService>((window, payload, service) => handler(window, payload.Message, payload.Origin, service));
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window closing is requested.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWindowClosingRequestedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowClosingRequested.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that determines whether the window should close.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">
    ///     The handler to invoke with the window and event args, returning a
    ///     <see cref="WindowClosingResult" />.
    /// </param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWindowClosingHandler<T>(this T obj, Func<IInfiniFrameWindow, EventArgs?, WindowClosingResult> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.Closing.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window is being created.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWindowCreatingHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowCreating.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window has been created.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWindowCreatedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowCreated.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler that is invoked when the window has been closed.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="handler">The handler to invoke with the window.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWindowClosedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WindowClosed.Add(handler);
        return obj;
    }

    /// <summary>
    ///     Registers user-defined custom schemes (other than 'http', 'https' and 'file') and handler methods to receive
    ///     callbacks
    ///     when the native browser control encounters them.
    /// </summary>
    /// <remarks>
    ///     Only 16 custom schemes can be registered before initialization. Additional handlers can be added after
    ///     initialization.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="builder">The builder of the window</param>
    /// <param name="scheme">The custom scheme</param>
    /// <param name="handler">
    ///     <see cref="EventHandler" />
    /// </param>
    /// <exception cref="ArgumentException">Thrown if no scheme or handler was provided</exception>
    /// <exception cref="ApplicationException">Thrown if more than 16 custom schemes were set</exception>
    public static IInfiniFrameWindowBuilder RegisterCustomSchemeHandler(this IInfiniFrameWindowBuilder builder, string scheme, Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler) {
        if (string.IsNullOrWhiteSpace(scheme)) throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");

        ArgumentNullException.ThrowIfNull(handler);

        string schemeLower = scheme.ToLower();

        if (builder.EventsStore.CustomScheme.Count > 15 && !builder.EventsStore.CustomScheme.ContainsKey(schemeLower))
            throw new ApplicationException("No more than 16 custom schemes can be set prior to initialization. Additional handlers can be added after initialization.");

        builder.EventsStore.CustomScheme.Add(schemeLower, handler);

        return builder;
    }

    /// <summary>
    ///     Registers a custom scheme handler on an already-initialized window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="scheme">The custom scheme name.</param>
    /// <param name="handler">The handler to invoke when the scheme is encountered.</param>
    /// <returns>The current <see cref="IInfiniFrameWindow" /> instance.</returns>
    public static IInfiniFrameWindow RegisterCustomSchemeHandler(this IInfiniFrameWindow window, string scheme, Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler) {
        if (string.IsNullOrWhiteSpace(scheme)) throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");

        string schemeLower = scheme.ToLower();

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger.Instance,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.AddCustomSchemeName,
            schemeLower
        );
        window.Events.EventsStore.CustomScheme.Add(schemeLower, handler);
        return window;
    }

    /// <summary>
    ///     Registers a handler for web message post data with a specific message identifier.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="messageId">The message identifier to listen for.</param>
    /// <param name="handler">The handler to invoke with the window and posted data.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessagePostHandler<T>(this T obj, string messageId, Action<IInfiniFrameWindow, string?> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessagePostData.Add(messageId, handler);
        return obj;
    }

    /// <summary>
    ///     Registers a handler for web message get data requests with a specific message identifier.
    /// </summary>
    /// <param name="obj">The object with an events store.</param>
    /// <param name="messageId">The message identifier to listen for.</param>
    /// <param name="handler">The handler to invoke with the window and request data, returning a response.</param>
    /// <typeparam name="T">The type of the object with an events store.</typeparam>
    /// <returns>The same instance for chaining.</returns>
    public static T RegisterWebMessageGetHandler<T>(this T obj, string messageId, Func<IInfiniFrameWindow, string?, string?> handler) where T : IHasInfiniFrameEventsStore {
        obj.EventsStore.WebMessageGetData.Add(messageId, handler);
        return obj;
    }

}
