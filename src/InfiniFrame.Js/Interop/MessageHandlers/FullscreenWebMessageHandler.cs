// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FullscreenWebMessageHandler {
    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.MessageHandlers.RegisterHandler(
            HandlerNames.FullscreenEnter,
            (window, _) => window.SetFullScreen(true)
        );
        
        builder.MessageHandlers.RegisterHandler(
            HandlerNames.FullscreenExit,
            (window, _) => window.SetFullScreen(false)
        );
        
        builder.MessageHandlers.RegisterHandler(
            HandlerNames.FullscreenToggle,
            (window, _) => window.SetFullScreen(!window.FullScreen)
        );

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterFullScreenChange);
        return builder;
    }
}
