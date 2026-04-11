// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Js.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public static class RegisterWindowCreatedUtility {
    private static readonly TimeSpan ReadyHandshakeTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromMilliseconds(100);
    private const int MaxSendAttempts = 3;
    
    private static readonly ConditionalWeakTable<IInfiniFrameWindowBuilder, ReadyRegistrationState> RegistrationStates = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow, string?> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, handler);
    }

    public static void RegisterMessageHandler(IInfiniFrameWindowBuilder builder, string messageId, Action<IInfiniFrameWindow> handler) {
        builder.MessageHandlers.RegisterMessageHandler(messageId, (w, _) => handler(w));
    }

    public static void RegisterWindowCreatedWebMessage(IInfiniFrameWindowBuilder builder, string messageId) {
        ReadyRegistrationState registrationState = RegistrationStates.GetOrCreateValue(builder);

        lock (registrationState) {
            registrationState.RegistrationMessageIds.Add(messageId);
        }

        EnsureWindowCreatedHandler(builder, registrationState);
        EnsureReadyHandler(builder, registrationState);
    }

    private static void EnsureWindowCreatedHandler(IInfiniFrameWindowBuilder builder, ReadyRegistrationState state) {
        lock (state) {
            if (state.WindowCreatedHandlerRegistered) return;
            state.WindowCreatedHandlerRegistered = true;
        }

        builder.Events.WindowCreated.Add(window => {
            WindowRegistrationState windowState;
            lock (state) {
                windowState = state.Windows.GetOrCreateValue(window);
                if (windowState.HandshakeTimeoutCancellationSource is not null)
                    return;

                windowState.HandshakeTimeoutCancellationSource = new CancellationTokenSource();
            }

            _ = MonitorReadyHandshakeTimeoutAsync(window, windowState);
        });
    }

    private static void EnsureReadyHandler(IInfiniFrameWindowBuilder builder, ReadyRegistrationState state) {
        lock (state) {
            if (state.ReadyHandlerRegistered) return;
            state.ReadyHandlerRegistered = true;
        }

        RegisterMessageHandler(builder, HandlerNames.WindowReady, (window, payload) => {
            WindowRegistrationState windowState;
            string[] registrationMessages;
            lock (state) {
                windowState = state.Windows.GetOrCreateValue(window);
                windowState.ReadyReceived = true;
                registrationMessages = state.RegistrationMessageIds.ToArray();
            }

            windowState.HandshakeTimeoutCancellationSource?.Cancel();
            windowState.HandshakeTimeoutCancellationSource?.Dispose();
            windowState.HandshakeTimeoutCancellationSource = null;

            if (windowState.RegistrationSent) return;
            windowState.RegistrationSent = true;

            _ = SendRegistrationsWithRetryAsync(window, registrationMessages);
        });
    }

    private static async Task MonitorReadyHandshakeTimeoutAsync(IInfiniFrameWindow window, WindowRegistrationState windowState) {
        CancellationTokenSource? timeoutSource = windowState.HandshakeTimeoutCancellationSource;
        if (timeoutSource is null) return;

        try {
            await Task.Delay(ReadyHandshakeTimeout, timeoutSource.Token);
            if (windowState.ReadyReceived) return;

            window.Logger.LogWarning(
                "Did not receive '{ReadyMessageId}' handshake within {TimeoutMs} ms; registration messages remain pending.",
                HandlerNames.WindowReady,
                ReadyHandshakeTimeout.TotalMilliseconds
            );
        }
        catch (OperationCanceledException) {
            // Handshake received in time.
        }
    }

    private static async Task SendRegistrationsWithRetryAsync(IInfiniFrameWindow window, IReadOnlyList<string> registrationMessages) {
        foreach (string registrationMessage in registrationMessages) {
            TimeSpan retryDelay = InitialRetryDelay;

            for (var attempt = 1; attempt <= MaxSendAttempts; attempt++) {
                try {
                    string envelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(registrationMessage);
                    await window.SendWebMessageAsync(envelope);
                    break;
                }
                catch (Exception ex) when (attempt < MaxSendAttempts) {
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
                catch (Exception ex) {
                    window.Logger.LogError(
                        ex,
                        "Failed to send registration message '{MessageId}' after {MaxAttempts} attempts.",
                        registrationMessage,
                        MaxSendAttempts
                    );
                }
            }
        }
    }
}
