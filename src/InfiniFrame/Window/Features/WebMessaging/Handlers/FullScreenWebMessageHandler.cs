// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods to register JavaScript handlers for entering, exiting, and toggling full-screen mode
///     from web content.
/// </summary>
public static class FullScreenWebMessageHandler {
    /// <summary>
    ///     Registers JavaScript message handlers for entering, exiting, and toggling full-screen mode.
    /// </summary>
    /// <typeparam name="T">The builder type.</typeparam>
    /// <param name="builder">The window builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenEnter,
            handler: (window, _) => window.Features.State.SetFullScreen()
        );

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenExit,
            handler: (window, _) => window.Features.State.SetFullScreen(false)
        );

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenToggle,
            handler: (window, _) => window.Features.State.SetFullScreen(!window.Features.State.IsFullScreen)
        );

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterFullScreenChange);
        return builder;
    }
}
