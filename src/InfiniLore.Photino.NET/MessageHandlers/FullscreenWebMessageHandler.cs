namespace InfiniLore.Photino.NET.MessageHandlers;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = "fullscreen:enter";
    private const string FullscreenExit = "fullscreen:exit";
    private const string FullscreenToggle = "fullscreen:toggle";

    public static void HandleWebMessage(object? sender, string? message) {
        if (sender is not IPhotinoWindow window) return;
        
        switch (message) {
            case FullscreenEnter: {
                window.SetFullScreen(true);
                return;
            }

            case FullscreenExit: {
                window.SetFullScreen(false);
                return;
            }

            case FullscreenToggle: {
                bool state = window.FullScreen;
                window.SetFullScreen(!state);
                return;
            }
        }
    }
}
