// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            HandlerNames.WindowMinimize,
            (window, _) => window.SetMinimized(true));

        builder.RegisterWebMessagePostHandler(
            HandlerNames.WindowMaximize,
            (window, _) => window.SetMaximized(true));

        builder.RegisterWebMessagePostHandler(
            HandlerNames.WindowClose,
            (window, _) => window.Close());

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterWindowClose);
        return builder;
    }
}
