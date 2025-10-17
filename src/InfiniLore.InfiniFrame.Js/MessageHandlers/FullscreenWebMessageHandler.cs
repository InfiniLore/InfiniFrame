// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using static InfiniLore.InfiniFrame.Js.Utilities.RegisterWindowCreatedUtility;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class FullscreenWebMessageHandler {
    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterMessageHandler(builder,
            HandlerNames.FullscreenEnter,
            static window => window.SetFullScreen(true));
        
        RegisterMessageHandler(builder,
            HandlerNames.FullscreenExit,
            static window => window.SetFullScreen(false));
        
        RegisterMessageHandler(builder,
            HandlerNames.FullscreenToggle,
            static window => window.SetFullScreen(!window.FullScreen));
        
        RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterFullScreenChange);
        return builder;
    }
}
