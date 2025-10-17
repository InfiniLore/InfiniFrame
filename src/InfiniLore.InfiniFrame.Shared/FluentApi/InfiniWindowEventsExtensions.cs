using System.Diagnostics.CodeAnalysis;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace InfiniLore.InfiniFrame;
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class InfiniWindowEventsExtensions {
    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when its location changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterLocationChangedHandler(this IInfiniFrameWindowBuilder builder, EventHandler<Point> handler) {
        builder.Events.WindowLocationChanged += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when its size changes.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterSizeChangedHandler(this IInfiniFrameWindowBuilder builder, EventHandler<Size> handler) {
        builder.Events.WindowSizeChanged += handler;
        return builder;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
    ///     in.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterFocusInHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowFocusIn += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is maximized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterMaximizedHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowMaximized += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is restored.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterRestoredHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowRestored += handler;
        return builder;
    }

    /// <summary>
    ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
    ///     out.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterFocusOutHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowFocusOut += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it is minimized.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterMinimizedHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowMinimized += handler;
        return builder;
    }


    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when it sends a message.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <remarks>
    ///     Messages can be sent from JavaScript via <code>builder.Events.external.sendMessage(message)</code>
    /// </remarks>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterWebMessageReceivedHandler(this IInfiniFrameWindowBuilder builder, EventHandler<string> handler) {
        builder.Events.WebMessageReceived += handler;
        return builder;
    }

    /// <summary>
    /// Registers user-defined handler methods to receive callbacks from the native builder before the window is closed through the native api calls.
    /// </summary>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterWindowClosingRequestedHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowClosingRequested += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks from the native builder when the builder is about to
    ///     close.
    ///     Handler can return true to prevent the builder from closing.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="NetClosingDelegate" /></param>
    public static IInfiniFrameWindowBuilder RegisterWindowClosingHandler(this IInfiniFrameWindowBuilder builder, NetClosingDelegate handler) {
        builder.Events.WindowClosing += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks before the native builder is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="EventHandler" /></param>
    public static IInfiniFrameWindowBuilder RegisterWindowCreatingHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowCreating += handler;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined handler methods to receive callbacks after the native builder is created.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindowBuilder" /> instance.
    /// </returns>
    /// <param name="builder">The builder to register the handler for.</param>
    /// <param name="handler"><see cref="NetClosingDelegate" /></param>
    public static IInfiniFrameWindowBuilder RegisterWindowCreatedHandler(this IInfiniFrameWindowBuilder builder, EventHandler handler) {
        builder.Events.WindowCreated += handler;
        return builder;
    }
}
