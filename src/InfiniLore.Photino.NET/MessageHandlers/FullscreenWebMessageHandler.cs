namespace InfiniLore.Photino.NET.MessageHandlers;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = "fullscreen:enter";
    private const string FullscreenExit = "fullscreen:exit";
    private const string FullscreenToggle = "fullscreen:toggle";

    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : IPhotinoWindowBuilder {
        builder.MessageHandlers.Register(FullscreenEnter, static (window, _) => window.SetFullScreen(true) );
        builder.MessageHandlers.Register(FullscreenExit, static (window, _) => window.SetFullScreen(false) );
        builder.MessageHandlers.Register(FullscreenToggle, static (window, _) => window.SetFullScreen(!window.FullScreen) );
        
        return builder;
    }
}
