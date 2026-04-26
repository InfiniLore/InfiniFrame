// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace InfiniFrame.Js.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class RegisterWindowCreatedUtility {
    private const int MaxSendAttempts = 3;
    private static readonly TimeSpan ReadyHandshakeTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromMilliseconds(100);

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

        builder.Events.WindowCreated.Add(window => {
            WindowRegistrationState windowState;
            lock (state.Lock) {
                windowState = state.Windows.GetOrCreateValue(window);
                if (windowState.HandshakeTimeoutCancellationSource is not null)
                    return;

                windowState.HandshakeTimeoutCancellationSource = new CancellationTokenSource();
            }

            _ = MonitorReadyHandshakeTimeoutAsync(window, state, windowState);
        });
    }

    private static void EnsureReadyHandler(IInfiniFrameWindowBuilder builder, WindowReadyRegistrationState state) {
        lock (state.Lock) {
            if (state.ReadyHandlerRegistered) return;

            state.ReadyHandlerRegistered = true;
        }

        builder.MessageHandlers.RegisterHandler(HandlerNames.WindowReady, handler: (window, payload) => {
            WindowRegistrationState windowState;
            string[] registrationMessages;
            lock (state.Lock) {
                windowState = state.Windows.GetOrCreateValue(window);
                if (!windowState.StateMachine.TryBeginRegistrationSendOnReady()) return;

                registrationMessages = state.RegistrationMessageIds.ToArray();
            }

            windowState.HandshakeTimeoutCancellationSource?.Cancel();
            windowState.HandshakeTimeoutCancellationSource?.Dispose();
            windowState.HandshakeTimeoutCancellationSource = null;

            _ = SendRegistrationsWithRetryAsync(window, state, windowState, registrationMessages);
        });
    }

    private static async Task MonitorReadyHandshakeTimeoutAsync(
        IInfiniFrameWindow window,
        WindowReadyRegistrationState state,
        WindowRegistrationState windowState
    ) {
        CancellationTokenSource? timeoutSource = windowState.HandshakeTimeoutCancellationSource;
        if (timeoutSource is null) return;

        try {
            await Task.Delay(ReadyHandshakeTimeout, timeoutSource.Token);
            lock (state.Lock) {
                if (!windowState.StateMachine.ShouldLogReadyHandshakeTimeout()) return;
            }

            window.Logger.LogWarning(
                "Did not receive '{ReadyMessageId}' handshake within {TimeoutMs} ms; registration messages remain pending.",
                HandlerNames.WindowReady,
                ReadyHandshakeTimeout.TotalMilliseconds
            );

            string[] registrationMessages;
            lock (state.Lock) {
                registrationMessages = state.RegistrationMessageIds.ToArray();
            }

            window.Logger.LogInformation(
                "Attempting fallback registration send before '{ReadyMessageId}' handshake. Pending message count: {RegistrationCount}.",
                HandlerNames.WindowReady,
                registrationMessages.Length
            );

            // Fallback send is intentionally decoupled from ready-state completion so an eventual
            // ready handshake can still trigger an additional registration send.
            _ = SendRegistrationsWithRetryAsync(window, state, windowState, registrationMessages, false);
        }
        catch (OperationCanceledException) {
            // Handshake received in time.
        }
    }

    private static async Task SendRegistrationsWithRetryAsync(
        IInfiniFrameWindow window,
        WindowReadyRegistrationState state,
        WindowRegistrationState windowState,
        IReadOnlyList<string> registrationMessages,
        bool completeStateOnFinish = true
    ) {
        bool allMessagesSent = false;
        try {
            allMessagesSent = await TrySendRegistrationsWithRetryAsync(window, registrationMessages);
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled error while sending window-created registration messages.");
        }
        finally {
            if (completeStateOnFinish) {
                lock (state.Lock) {
                    windowState.StateMachine.CompleteRegistrationSend(allMessagesSent);
                }
            }
        }
    }

    private static async Task<bool> TrySendRegistrationsWithRetryAsync(IInfiniFrameWindow window, IReadOnlyList<string> registrationMessages) {
        bool allMessagesSent = true;
        foreach (string registrationMessage in registrationMessages) {
            TimeSpan retryDelay = InitialRetryDelay;
            bool messageSent = false;

            for (int attempt = 1; attempt <= MaxSendAttempts; attempt++) {
                try {
                    string envelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(registrationMessage);
                    await window.SendWebMessageAsync(envelope);
                    messageSent = true;
                    break;
                }
                catch (OperationCanceledException) {
                    throw;
                }
                catch (InvalidOperationException ex) when (attempt < MaxSendAttempts) {
                    window.Logger.LogWarning(
                        ex,
                        "Failed to send registration message '{MessageId}' on attempt {Attempt}/{MaxAttempts}; retrying in {DelayMs} ms.",
                        registrationMessage,
                        attempt,
                        MaxSendAttempts,
                        retryDelay.TotalMilliseconds
                    );
                    await Task.Delay(retryDelay);
                    retryDelay += retryDelay;
                }
                catch (InvalidOperationException ex) {
                    window.Logger.LogError(
                        ex,
                        "Failed to send registration message '{MessageId}' after {MaxAttempts} attempts.",
                        registrationMessage,
                        MaxSendAttempts
                    );
                }
            }

            if (!messageSent)
                allMessagesSent = false;
        }

        return allMessagesSent;
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
