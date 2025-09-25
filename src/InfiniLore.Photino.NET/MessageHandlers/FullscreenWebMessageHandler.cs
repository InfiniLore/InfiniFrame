namespace InfiniLore.Photino.NET.MessageHandlers;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = "fullscreen:enter";
    private const string FullscreenExit = "fullscreen:exit";
    private const string FullscreenToggle = "fullscreen:toggle";

    public static bool TryHandleWebMessage(IPhotinoWindow window, string? message) {
        switch (message) {
            case FullscreenEnter: {
                window.SetFullScreen(true);
                return true;
            }

            case FullscreenExit: {
                window.SetFullScreen(false);
                return true;
            }

            case FullscreenToggle: {
                bool state = window.FullScreen;
                window.SetFullScreen(!state);
                return true;
            }
        }
        return false;
    }
}
