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
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class InfiniWindowEventsExtensions {
    /// <param name="builder">The builder to register the handler for.</param>
    extension<T>(T builder) where T : IHasInfiniFrameEvents {
        /// <summary>
        ///     Registers user-defined handler methods to receive callbacks from the native builder when its location changes.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
        /// </returns>
        /// <param name="handler">Handler invoked with the window and new location.</param>
        public T RegisterLocationChangedHandler(Action<IInfiniFrameWindow, Point> handler) {
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
        public T RegisterSizeChangedHandler(Action<IInfiniFrameWindow, Size> handler) {
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
        public T RegisterFocusInHandler(Action<IInfiniFrameWindow> handler) {
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
        public T RegisterMaximizedHandler(Action<IInfiniFrameWindow> handler) {
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
        public T RegisterRestoredHandler(Action<IInfiniFrameWindow> handler) {
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
        public T RegisterFocusOutHandler(Action<IInfiniFrameWindow> handler) {
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
        public T RegisterMinimizedHandler(Action<IInfiniFrameWindow> handler) {
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
        ///     Messages can be sent from JavaScript via <code>builder.Events.external.sendMessage(message)</code>
        /// </remarks>
        /// <param name="handler">Handler invoked with the window and message.</param>
        public T RegisterWebMessageReceivedHandler(Action<IInfiniFrameWindow, string> handler) {
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
        ///     Messages can be sent from JavaScript via <code>builder.Events.external.sendMessage(message)</code>
        /// </remarks>
        /// <param name="handler">Handler that receives the resolved service and web message data.</param>
        public T RegisterWebMessageReceivedHandler<TService>(Action<TService, IInfiniFrameWindow, string> handler) where TService : notnull {
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
        /// Registers user-defined handler methods to receive callbacks from the native builder before the window is closed through the native api calls.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IHasInfiniFrameEvents" /> instance.
        /// </returns>
        /// <param name="handler">Handler invoked with the window.</param>
        public T RegisterWindowClosingRequestedHandler(Action<IInfiniFrameWindow> handler) {
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
        /// <param name="handler"><see cref="NetClosingDelegate" /></param>
        public T RegisterWindowClosingHandler(NetClosingDelegate handler) {
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
        public T RegisterWindowCreatingHandler(Action<IInfiniFrameWindow> handler) {
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
        public T RegisterWindowCreatedHandler(Action<IInfiniFrameWindow> handler) {
            builder.Events.WindowCreated.Add(handler);
            return builder;
        }
    }

}
