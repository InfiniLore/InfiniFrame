// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;
using Microsoft.Extensions.Logging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangedWebMessageHandler {
    private const string TitleChanged = HandlerNames.InfiniWindowPrefix + "title:change";
    private const string RegisterTitleChange = HandlerNames.InfiniWindowPrefix + "register:title:change";

    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : class, IPhotinoWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(TitleChanged, HandleWebMessage);
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterTitleChange);
        return builder;
    }
    
    private static void HandleWebMessage(IPhotinoWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;
        window.Logger.LogInformation("title:change {payload}", payload);
        window.SetTitle(payload);
    }
    
}
