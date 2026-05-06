// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtility {
    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, WindowReadyRegistrationState> RegistrationStates = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static void RegisterWindowCreatedWebMessage(IInfiniFrameWindowBuilder builder, string messageId) {
        WindowReadyRegistrationState registrationState = RegistrationStates.GetOrCreateValue(builder);

        lock (registrationState.Lock) {
            registrationState.RegistrationMessageIds.Add(messageId);
        }

        EnsureWindowCreatedHandler(builder, registrationState);
        EnsureReadyHandler(builder, registrationState);
    }

    private static void EnsureWindowCreatedHandler(IInfiniFrameWindowBuilder builder, WindowReadyRegistrationState state) {
        lock (state.Lock) {
            if (state.WindowCreatedHandlerRegistered) return;

            state.WindowCreatedHandlerRegistered = true;
        }

        builder.EventsStore.WindowCreated.Add(window => {
            lock (state.Lock) {
                state.Windows.GetOrCreateValue(window);
            }
        });
    }

    private static void EnsureReadyHandler(IInfiniFrameWindowBuilder builder, WindowReadyRegistrationState state) {
        lock (state.Lock) {
            if (state.ReadyHandlerRegistered) return;

            state.ReadyHandlerRegistered = true;
        }

        builder.RegisterWebMessagePostHandler(JsHandlerNames.WindowReady, handler: (window, payload) => {
            WindowRegistrationState windowState;
            string[] registrationMessages;
            lock (state.Lock) {
                windowState = state.Windows.GetOrCreateValue(window);
                if (!windowState.StateMachine.TryBeginRegistrationSendOnReady()) return;

                registrationMessages = state.RegistrationMessageIds.ToArray();
            }

            window.Logger.LogDebug(
                "Received '{ReadyMessageId}' handshake. Sending {RegistrationCount} registration messages before acknowledgement.",
                JsHandlerNames.WindowReady,
                registrationMessages.Length
            );

            _ = SendRegistrationsAndAckAsync(window, state, windowState, registrationMessages);
        });
    }

    private static async Task SendRegistrationsAndAckAsync(
        IInfiniFrameWindow window,
        WindowReadyRegistrationState state,
        WindowRegistrationState windowState,
        IReadOnlyList<string> registrationMessages
    ) {
        bool allMessagesSent = false;
        try {
            allMessagesSent = await SendRegistrationsAndAckAsync(window, registrationMessages);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled error while sending window-created registration messages.");
        }
        finally {
            lock (state.Lock) {
                windowState.StateMachine.CompleteRegistrationSend(allMessagesSent);
            }
        }
    }

    private static async Task<bool> SendRegistrationsAndAckAsync(IInfiniFrameWindow window, IReadOnlyList<string> registrationMessages) {
        foreach (string registrationMessage in registrationMessages) {
            string envelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(registrationMessage);
            await window.SendWebMessageAsync(envelope);
        }

        await window.SendWebMessageAsync(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WindowReadyAck));
        window.Logger.LogDebug("Sent '{ReadyAckMessageId}' handshake acknowledgement.", JsHandlerNames.WindowReadyAck);
        return true;
    }
}
