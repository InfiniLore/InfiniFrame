// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMinimize,
            (window, _) => window.SetMinimized(true));

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMaximize,
            (window, _) => window.SetMaximized(true));

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowClose,
            (window, _) => window.Close());

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterWindowClose);
        return builder;
    }
}
