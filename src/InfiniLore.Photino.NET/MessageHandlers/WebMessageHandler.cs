// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WebMessageHandler {
    public static bool TryHandleWebMessage(object? sender, string? message, params Func<IPhotinoWindow, string?, bool>[] handlers) {
        if (sender is not IPhotinoWindow window) return false;

        if (handlers.Any(handler => handler(window,message))) return true;
        return false;
    }
}
