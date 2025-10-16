using InfiniLore.Photino.NET;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;

public static class WindowManagementWebMessageHandler {
    private const string WindowMinimize = HandlerNames.InfiniWindowPrefix + "window:minimize";
    private const string WindowMaximize = HandlerNames.InfiniWindowPrefix + "window:maximize";
    private const string WindowClose = HandlerNames.InfiniWindowPrefix + "window:close";
    // private const string WindowOpen = HandlerNames.InfiniWindowPrefix + "window:open";
    
    private const string RegisterWindowClose = HandlerNames.InfiniWindowPrefix + "register:window:close";
    // private const string RegisterWindowOpen = HandlerNames.InfiniWindowPrefix + "register:window:open";

    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IPhotinoWindowBuilder {
        builder.MessageHandlers.RegisterMessageHandler(WindowMinimize, static (window, _) => window.SetMinimized(true) );
        builder.MessageHandlers.RegisterMessageHandler(WindowMaximize, static (window, _) => window.SetMaximized(true) );
        builder.MessageHandlers.RegisterMessageHandler(WindowClose, static (window, _) => window.Close() );
        
        RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterWindowClose);
        // RegisterWindowCreatedUtilities.RegisterWindowCreatedWebMessage(builder, RegisterWindowOpen);
        return builder;
    }
}
