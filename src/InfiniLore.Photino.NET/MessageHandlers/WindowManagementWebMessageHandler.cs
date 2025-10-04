namespace InfiniLore.Photino.NET.MessageHandlers;
public static class WindowManagementWebMessageHandler {
    private const string WindowMinimize = HandlerNames.InfiniWindowPrefix + "window:minimize";
    private const string WindowMaximize = HandlerNames.InfiniWindowPrefix + "window:maximize";
    private const string WindowClose = HandlerNames.InfiniWindowPrefix + "window:close";

    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IPhotinoWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(WindowMinimize, static (window, _) => window.SetMinimized(true) );
        builder.MessageHandlers.RegisterMessageHandler(WindowMaximize, static (window, _) => window.SetMaximized(true) );
        builder.MessageHandlers.RegisterMessageHandler(WindowClose, static (window, _) => window.Close() );
        
        return builder;
    }
}
