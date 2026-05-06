// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FullscreenWebMessageHandler {
    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenEnter,
            (window, _) => window.SetFullScreen(true)
        );
        
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenExit,
            (window, _) => window.SetFullScreen(false)
        );
        
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.FullscreenToggle,
            (window, _) => window.SetFullScreen(!window.FullScreen)
        );

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterFullScreenChange);
        return builder;
    }
}
