// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using static InfiniLore.InfiniFrame.Js.Utilities.RegisterWindowCreatedUtility;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangedWebMessageHandler {
    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterMessageHandler(builder, HandlerNames.TitleChanged, HandleWebMessage);
        RegisterWindowCreatedWebMessage(builder, HandlerNames.RegisterTitleChange);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        window.Logger.LogInformation("title:change {payload}", payload);
        window.SetTitle(payload);
    }

}
