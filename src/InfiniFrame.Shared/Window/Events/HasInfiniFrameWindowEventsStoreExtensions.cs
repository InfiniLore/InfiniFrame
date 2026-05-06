// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class HasInfiniFrameWindowEventsStoreExtensions {
    public static T RegisterLocationChangedHandler<T>(this T obj, Action<IInfiniFrameWindow, Point> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowLocationChanged.Add(handler);
        return obj;
    }

    public static T RegisterSizeChangedHandler<T>(this T obj, Action<IInfiniFrameWindow, Size> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowSizeChanged.Add(handler);
        return obj;
    }

    public static T RegisterFocusInHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowFocusIn.Add(handler);
        return obj;
    }

    public static T RegisterMaximizedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowMaximized.Add(handler);
        return obj;
    }

    public static T RegisterRestoredHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowRestored.Add(handler);
        return obj;
    }

    public static T RegisterFocusOutHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowFocusOut.Add(handler);
        return obj;
    }

    public static T RegisterMinimizedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowMinimized.Add(handler);
        return obj;
    }

    public static T RegisterWebMessageReceivedHandler<T>(this T obj, Action<IInfiniFrameWindow, string> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessageReceived.Add((window, payload) => handler(window, payload.Message));
        return obj;
    }

    public static T RegisterWebMessageReceivedHandler<T, TService>(this T obj, Action<IInfiniFrameWindow, string, TService> handler) where TService : notnull where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessageReceived.AddWithServiceResolving<TService>((window, payload, service) => handler(window, payload.Message, service));
        return obj;
    }

    public static T RegisterWebMessageReceivedHandler<T>(this T obj, Action<IInfiniFrameWindow, string, string?> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessageReceived.Add((window, payload) => handler(window, payload.Message, payload.Origin));
        return obj;
    }

    public static T RegisterWebMessageReceivedHandler<T, TService>(this T obj, Action<IInfiniFrameWindow, string, string?, TService> handler) where TService : notnull where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessageReceived.AddWithServiceResolving<TService>((window, payload, service) => handler(window, payload.Message, payload.Origin, service));
        return obj;
    }

    public static T RegisterWindowClosingRequestedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowClosingRequested.Add(handler);
        return obj;
    }

    public static T RegisterWindowClosingHandler<T>(this T obj, Func<IInfiniFrameWindow, EventArgs?, WindowClosingResult> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowClosing.Add(handler);
        return obj;
    }

    public static T RegisterWindowCreatingHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowCreating.Add(handler);
        return obj;
    }

    public static T RegisterWindowCreatedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WindowCreated.Add(handler);
        return obj;
    }

    public static T RegisterWindowClosedHandler<T>(this T obj, Action<IInfiniFrameWindow> handler) where T : IHasInfiniFrameWindowEventsStore {
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
    
    public static IInfiniFrameWindow RegisterCustomSchemeHandler(this IInfiniFrameWindow window, string scheme, Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)> handler) {
        if (string.IsNullOrWhiteSpace(scheme)) throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");
        string schemeLower = scheme.ToLower();
        
        InfiniFrameNative.AddCustomSchemeName(window.InstanceHandle, schemeLower);
        window.Events.EventsStore.CustomScheme.Add(schemeLower, handler);
        return window;
    }
    
    public static T RegisterWebMessagePostHandler<T>(this T obj, string messageId, Action<IInfiniFrameWindow, string?> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessagePostData.Add(messageId, handler);
        return obj;
    }

    public static T RegisterWebMessageGetHandler<T>(this T obj, string messageId, Func<IInfiniFrameWindow, string?, string?> handler) where T : IHasInfiniFrameWindowEventsStore {
        obj.EventsStore.WebMessageGetData.Add(messageId, handler);
        return obj;
    }

}
