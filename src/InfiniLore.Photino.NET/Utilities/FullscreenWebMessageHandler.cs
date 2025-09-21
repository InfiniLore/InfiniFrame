namespace InfiniLore.Photino.NET.Utilities;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = "fullscreen:enter";
    private const string FullscreenExit = "fullscreen:exit";

    public static bool TryHandleWebMessage(object? sender, string? message) {
        if (sender is not IPhotinoWindow window) return false;

        switch (message) {
            case FullscreenEnter: {
                window.SetFullScreen(true);
                return true;
            }

            case FullscreenExit: {
                window.SetFullScreen(false);
                return true;
            }
        }
        return false;
    }
}
