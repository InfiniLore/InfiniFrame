// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for registering web messages that are sent automatically when a window is created and
///     ready.
/// </summary>
public static class RegisterWindowCreatedUtility {
    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, WindowReadyRegistrationState> RegistrationStates = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Registers a web message to be sent to a new window when it signals readiness.
    /// </summary>
    /// <param name="builder">The window builder associated with the registration.</param>
    /// <param name="messageId">The identifier of the message to send to the window on ready.</param>
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

            ILogger? logger = window.ServiceProvider?.GetService<ILoggerFactory>()?.CreateLogger(typeof(RegisterWindowCreatedUtility));
            _ = Task.Run(async () => {
                try {
                    await SendRegistrationsAndAckAsync(window, state, windowState, registrationMessages).ConfigureAwait(false);
                }
                catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                    logger?.LogWarning(ex, "Unhandled error while sending window-created registration messages.");
                }
            });
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
            allMessagesSent = await SendRegistrationsAndAckAsync(window, registrationMessages).ConfigureAwait(false);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            // Non-fatal: registration messages may fail if the window is closing.
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
            await window.SendWebMessageAsync(envelope).ConfigureAwait(false);
        }

        await window.SendWebMessageAsync(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WindowReadyAck)).ConfigureAwait(false);
        return true;
    }
}
