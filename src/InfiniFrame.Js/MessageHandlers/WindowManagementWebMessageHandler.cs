// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.WindowMinimize,
            static window => window.SetMinimized(true));

        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.WindowMaximize,
            static window => window.SetMaximized(true));

        RegisterWindowCreatedUtility.RegisterMessageHandler(builder,
            HandlerNames.WindowClose,
            static window => window.Close());

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterWindowClose);
        return builder;
    }
}
