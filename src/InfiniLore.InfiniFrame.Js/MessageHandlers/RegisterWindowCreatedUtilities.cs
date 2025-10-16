// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace InfiniLore.InfiniFrame.Js.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtilities {
    public static void RegisterWindowCreatedWebMessage(IInfiniWindowBuilder builder, string messageId) {
        builder.Events.WindowCreated += (sender, args) => {
            if (sender is not IInfiniWindow window) return;

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
