// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Microsoft.Extensions.Logging;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangedWebMessageHandler {
    private const string TitleChanged = HandlerNames.InfiniWindowPrefix + "title:change";
    private const string RegisterTitleChange = HandlerNames.InfiniWindowPrefix + "register:title:change";

    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(TitleChanged, HandleWebMessage);
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterTitleChange);
        return builder;
    }

    private static void HandleWebMessage(IInfiniFrameWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        window.Logger.LogInformation("title:change {payload}", payload);
        window.SetTitle(payload);
    }

}
