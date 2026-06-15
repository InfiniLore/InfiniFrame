// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMinimize,
            (window, _) => window.Features.State.SetMinimized());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMaximize,
            (window, _) => window.Features.State.SetMaximized());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowClose,
            (window, _) => window.Features.Lifecycle.Close());

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterWindowClose);
        return builder;
    }
}
