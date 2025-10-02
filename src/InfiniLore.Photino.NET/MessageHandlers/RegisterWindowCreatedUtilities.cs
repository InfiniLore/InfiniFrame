// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET.MessageHandlers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtilities {
    public static void RegisterWindowCreatedWebMessage(IPhotinoWindowBuilder builder, string messageId) {
        builder.Events.WindowCreated += (sender, args) => {
            if (sender is not IPhotinoWindow window) return;

            // TODO this is a hack but works because we can only send an event after the window is fully created.
            //      The issue is that OnWindowCreated is called before the window is fully finalized.
            _ = Task.Run(async () => {
                await Task.Delay(1000);
                await window.SendWebMessageAsync(messageId);
            });
        };
    }
    
}
