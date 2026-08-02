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
            (window, _) => window.Features.State.SetFullScreen()
        );

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenExit,
            (window, _) => window.Features.State.SetFullScreen(false)
        );

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenToggle,
            (window, _) => window.Features.State.SetFullScreen(!window.Features.State.IsFullScreen)
        );

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterFullScreenChange);
        return builder;
    }
}