using InfiniLore.Photino.NET;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
public static class FullscreenWebMessageHandler {
    private const string FullscreenEnter = HandlerNames.InfiniWindowPrefix + "fullscreen:enter";
    private const string FullscreenExit = HandlerNames.InfiniWindowPrefix + "fullscreen:exit";
    private const string FullscreenToggle = HandlerNames.InfiniWindowPrefix + "fullscreen:toggle";

    private const string RegisterFullScreenChange = HandlerNames.InfiniWindowPrefix + "register:fullscreen:change";

    public static T RegisterFullScreenWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(FullscreenEnter, handler: static (window, _) => window.SetFullScreen(true));
        builder.MessageHandlers.RegisterMessageHandler(FullscreenExit, handler: static (window, _) => window.SetFullScreen(false));
        builder.MessageHandlers.RegisterMessageHandler(FullscreenToggle, handler: static (window, _) => window.SetFullScreen(!window.FullScreen));

        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterFullScreenChange);
        return builder;
    }
}
