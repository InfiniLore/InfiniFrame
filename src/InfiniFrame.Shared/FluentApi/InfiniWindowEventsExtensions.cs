// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class InfiniWindowEventsExtensions {
    /// <param name="builder">The builder to register the handler for.</param>
    extension<T>(T builder) where T : IHasInfiniFrameEvents {
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when its location changes.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterLocationChangedHandler(EventHandler<Point> handler) {
            builder.Events.WindowLocationChanged += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when its size changes.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterSizeChangedHandler(EventHandler<Size> handler) {
            builder.Events.WindowSizeChanged += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
        ///     in.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterFocusInHandler(EventHandler handler) {
            builder.Events.WindowFocusIn += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when it is maximized.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterMaximizedHandler(EventHandler handler) {
            builder.Events.WindowMaximized += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when it is restored.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterRestoredHandler(EventHandler handler) {
            builder.Events.WindowRestored += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers registered user-defined handler methods to receive callbacks from the native builder when it is focused
        ///     out.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterFocusOutHandler(EventHandler handler) {
            builder.Events.WindowFocusOut += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when it is minimized.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterMinimizedHandler(EventHandler handler) {
            builder.Events.WindowMinimized += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when it sends a message.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <remarks>
        ///     Messages can be sent from JavaScript via <code>builder.Events.external.sendMessage(message)</code>
        /// </remarks>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterWebMessageReceivedHandler(EventHandler<string> handler) {
            builder.Events.WebMessageReceived += handler;
            return builder;
        }
        
        /// <summary>
        /// Registers user-defined handler methods to receive callbacks from the native builder before the window is closed through the native api calls.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterWindowClosingRequestedHandler(EventHandler handler) {
            builder.Events.WindowClosingRequested += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when the builder is about to
        ///     close.
        ///     Handler can return true to prevent the builder from closing.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="NetClosingDelegate" /></param>
        public T RegisterWindowClosingHandler(NetClosingDelegate handler) {
            builder.Events.WindowClosing += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks before the native builder is created.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="EventHandler" /></param>
        public T RegisterWindowCreatingHandler(EventHandler handler) {
            builder.Events.WindowCreating += handler;
            return builder;
        }
        
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks after the native builder is created.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameProperties" /> instance.
        /// </returns>
        /// <param name="handler"><see cref="NetClosingDelegate" /></param>
        public T RegisterWindowCreatedHandler(EventHandler handler) {
            builder.Events.WindowCreated += handler;
            return builder;
        }
    }
}
