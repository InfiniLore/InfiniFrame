// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;
using Microsoft.Extensions.Logging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangedWebMessageHandler {
    private const string TitleChanged = "title:change";

    public static T RegisterTitleChangedWebMessageHandler<T>(this T builder) where T : IPhotinoWindowBuilder {
        builder.MessageHandlers.Register(TitleChanged, HandleWebMessage);
        return builder;
    }
    
    private static void HandleWebMessage(IPhotinoWindow window, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;
        window.Logger.LogInformation("title:change {payload}", payload);
        window.SetTitle(payload);
    }
    
}
