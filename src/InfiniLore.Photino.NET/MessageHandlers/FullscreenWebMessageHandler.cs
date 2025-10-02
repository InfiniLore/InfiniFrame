namespace InfiniLore.Photino.NET.MessageHandlers;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = "fullscreen:enter";
    private const string FullscreenExit = "fullscreen:exit";
    private const string FullscreenToggle = "fullscreen:toggle";
    
    private const string RegisterFullScreenChange = "register:fullscreen:change";

    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IPhotinoWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(FullscreenEnter, static (window, _) => window.SetFullScreen(true) );
        builder.MessageHandlers.RegisterMessageHandler(FullscreenExit, static (window, _) => window.SetFullScreen(false) );
        builder.MessageHandlers.RegisterMessageHandler(FullscreenToggle, static (window, _) => window.SetFullScreen(!window.FullScreen) );
        
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterFullScreenChange);
        return builder;
    }
}
