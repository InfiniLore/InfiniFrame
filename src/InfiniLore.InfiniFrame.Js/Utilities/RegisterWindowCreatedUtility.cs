// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Js.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtility {
    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow, string?> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, handler);
    }
    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, (w, _) => handler(w));
    }
    
    public static void RegisterWindowCreatedWebMessage(IInfiniFrameWindowBuilder builder, string messageId) {
        builder.Events.WindowCreated += (sender, args) => {
            if (sender is not IInfiniFrameWindow window) return;

            // TODO this is a hack but works because we can only send an event after the window is fully created.
            //      The issue is that OnWindowCreated is called before the window is fully finalized.
            //      We should fix this in the future.
            _ = Task.Run(async () => {
                await Task.Delay(1000);
                await window.SendWebMessageAsync(messageId);
            });
        };
    }

}
