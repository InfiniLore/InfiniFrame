// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangedWebMessageHandler {
    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(JsHandlerNames.TitleChanged, HandleWebMessage);
        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterTitleChange);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        window.Logger.LogInformation("title:change {payload}", payload);
        window.SetTitle(payload);
    }
}
