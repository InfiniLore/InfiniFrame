// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FullScreenWebMessageHandler {
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
