// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js.Interop.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.MessageHandlers.RegisterHandler(
            HandlerNames.WindowMinimize,
            (window, _) => window.SetMinimized(true));

        builder.MessageHandlers.RegisterHandler(
            HandlerNames.WindowMaximize,
            (window, _) => window.SetMaximized(true));

        builder.MessageHandlers.RegisterHandler(
            HandlerNames.WindowClose,
            (window, _) => window.Close());

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterWindowClose);
        return builder;
    }
}
