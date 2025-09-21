using System.Drawing;

namespace InfiniLore.Photino.NET;
public static class PhotinoWindowEventsExtensions {
    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when its location changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterLocationChangedHandler(this IPhotinoWindow window, EventHandler<Point> handler) {
        window.WindowLocationChanged += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when its size changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterSizeChangedHandler(this IPhotinoWindow window, EventHandler<Size> handler) {
        window.WindowSizeChanged += handler;
        return window;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native window when it is focused
    ///     in.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterFocusInHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowFocusIn += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when it is maximized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterMaximizedHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowMaximized += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when it is restored.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterRestoredHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowRestored += handler;
        return window;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native window when it is focused
    ///     out.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterFocusOutHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowFocusOut += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when it is minimized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterMinimizedHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowMinimized += handler;
        return window;
    }


    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when it sends a message.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Messages can be sent from JavaScript via <code>window.external.sendMessage(message)</code>
    /// </remarks>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterWebMessageReceivedHandler(this IPhotinoWindow window, EventHandler<string> handler) {
        window.WebMessageReceived += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native window when the window is about to
    ///     close.
    ///     Handler can return true to prevent the window from closing.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="NetClosingDelegate" /></param>
    public static IPhotinoWindow RegisterWindowClosingHandler(this IPhotinoWindow window, NetClosingDelegate handler) {
        window.WindowClosing += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks before the native window is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IPhotinoWindow RegisterWindowCreatingHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowCreating += handler;
        return window;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks after the native window is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">The window to register the handler for.</param>
    /// <param name="handler"><see cref="NetClosingDelegate" /></param>
    public static IPhotinoWindow RegisterWindowCreatedHandler(this IPhotinoWindow window, EventHandler handler) {
        window.WindowCreated += handler;
        return window;
    }
}
