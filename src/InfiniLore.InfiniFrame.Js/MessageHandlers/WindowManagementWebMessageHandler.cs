// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using static InfiniLore.InfiniFrame.Js.Utilities.RegisterWindowCreatedUtility;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterMessageHandler(builder,
            HandlerNames.WindowMinimize,
            static window => window.SetMinimized(true));
        
        RegisterMessageHandler(builder,
            HandlerNames.WindowMaximize,
            static window => window.SetMaximized(true));
        
        RegisterMessageHandler(builder,
            HandlerNames.WindowClose,
            static window => window.Close());

        RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterWindowClose);
        
        // RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterWindowOpen);
        return builder;
    }
}
