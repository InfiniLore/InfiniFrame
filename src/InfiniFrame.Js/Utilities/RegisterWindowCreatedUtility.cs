// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using InfiniFrame.Interop;

namespace InfiniFrame.Js.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtility {
    private sealed class ReadyRegistrationState {
        public bool Registered { get; set; }
    }

    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, ReadyRegistrationState> ReadyRegistrations = new();
    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow, string?> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, handler);
    }
    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, (w, _) => handler(w));
    }

    public static void RegisterWindowCreatedWebMessage(IInfiniFrameWindowBuilder builder, string messageId) {
        if (TryRegisterReadyHandler(builder)) {
            RegisterMessageHandler(builder, HandlerNames.WindowReady, (window, payload) => {
                _ = window.SendWebMessageAsync(InteropEnvelopeProtocol.CreateEnvelopeMessage(messageId));
            });
        }

        builder.Events.WindowCreated.Add(window => {
            // TODO this is a hack but works because we can only send an event after the window is fully created.
            //      The issue is that OnWindowCreated is called before the window is fully finalized.
            //      We should fix this in the future.
            _ = Task.Run(async () => {
                await Task.Delay(1000);
                await window.SendWebMessageAsync(InteropEnvelopeProtocol.CreateEnvelopeMessage(messageId));
            });
        });
    }

    private static bool TryRegisterReadyHandler(IInfiniFrameWindowBuilder builder) {
        ReadyRegistrationState state = ReadyRegistrations.GetOrCreateValue(builder);
        lock (state) {
            if (state.Registered) return false;

            state.Registered = true;
            return true;
        }
    }

}
