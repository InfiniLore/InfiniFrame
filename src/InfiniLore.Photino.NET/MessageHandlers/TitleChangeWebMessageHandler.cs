// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;
using Microsoft.Extensions.Logging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class TitleChangeWebMessageHandler {
    private const string TitleChanged = "title:change";

    public static void HandleWebMessage(object? sender, string? message) {
        if (sender is not IPhotinoWindow window) return;
        string[]? split = message?.Split(';',2, StringSplitOptions.RemoveEmptyEntries);
        switch (split?.FirstOrDefault()) {
            case TitleChanged: {
                string? title = split.ElementAtOrDefault(1);
                if (string.IsNullOrWhiteSpace(title)) break;
                window.Logger.LogInformation("title:change {title}", title);
                window.SetTitle(title);
                break;
            }
        }
    }
    
}
