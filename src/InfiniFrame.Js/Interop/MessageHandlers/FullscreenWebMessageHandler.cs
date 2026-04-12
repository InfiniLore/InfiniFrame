// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FullscreenWebMessageHandler {
    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.FullscreenEnter,
            static window => window.SetFullScreen(true));

        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.FullscreenExit,
            static window => window.SetFullScreen(false));

        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.FullscreenToggle,
            static window => window.SetFullScreen(!window.FullScreen));

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterFullScreenChange);
        return builder;
    }
}
