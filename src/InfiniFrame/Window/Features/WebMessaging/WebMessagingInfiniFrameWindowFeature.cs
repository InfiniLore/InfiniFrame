// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WebMessagingInfiniFrameWindowFeature : IWebMessagingInfiniFrameWindowFeature {
    private static long _nextAcknowledgementId;
    private readonly IInfiniFrameWindow window;
    private readonly ILogger<WebMessagingInfiniFrameWindowFeature> logger;
    private readonly ConcurrentDictionary<ulong, TaskCompletionSource> _acknowledgements = new();

    public WebMessagingInfiniFrameWindowFeature(
        IInfiniFrameWindow window,
        ILogger<WebMessagingInfiniFrameWindowFeature> logger
    ) {
        this.window = window;
        this.logger = logger;
        window.EventsStore.WebMessagePostData.Add(
            JsHandlerNames.WebMessageAckResponse,
            (_, payload) => {
                if (ulong.TryParse(payload, out ulong id) && _acknowledgements.TryRemove(id, out TaskCompletionSource? completion))
                    completion.TrySetResult();
            }
        );
    }
    /// <inheritdoc cref="IWebMessagingInfiniFrameWindowFeature.SendWebMessage" />
    public void SendWebMessage(string message) {
        if (window.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SendWebMessage,
            message
        );
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement
    /// <inheritdoc cref="IWebMessagingInfiniFrameWindowFeature.SendWebMessageAsync" />
    public ValueTask SendWebMessageAsync(string message, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(message);
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);
        return SendLocallyAsync(message, ct);
    }

    private async ValueTask SendLocallyAsync(string message, CancellationToken ct) {
        if (window.IsClosedOrClosing()) return;

        InfiniFrameDispatchResult result = await window.DispatchAsync(
            () => {
                using var lease = window.AcquireNativeHandle();
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.SendWebMessage(lease.Handle, message);
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new ApplicationException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not submit web message.");
            },
            cancellationToken: ct
        ).ConfigureAwait(false);

        if (result == InfiniFrameDispatchResult.Cancelled)
            throw new OperationCanceledException(ct);
        if (result is InfiniFrameDispatchResult.Failed or InfiniFrameDispatchResult.TimedOut)
            throw new InvalidOperationException($"Web-message submission ended with {result}.");
    }

    public async Task SendWebMessageWithAcknowledgementAsync(string message, CancellationToken ct = default) {
        ArgumentNullException.ThrowIfNull(message);
        ct.ThrowIfCancellationRequested();
        ObjectDisposedException.ThrowIf(window.IsClosedOrClosing(), window.GetType().Name);

        ulong id = unchecked((ulong)Interlocked.Increment(ref _nextAcknowledgementId));
        string? diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation("WebMessageAcknowledgement", id);
        string finalState = "Failed";
        string? failureReason = null;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_acknowledgements.TryAdd(id, completion))
            throw new InvalidOperationException("Could not register the web-message acknowledgement.");

        try {
            string payload = CreateAcknowledgementPayload(id, message);
            string envelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WebMessageAckRequest, payload);
            await SendLocallyAsync(envelope, ct).ConfigureAwait(false);

            Task closed = window.WaitForCloseAsync().AsTask();
            Task terminal = await Task.WhenAny(completion.Task, closed).WaitAsync(ct).ConfigureAwait(false);
            if (terminal == closed)
                throw new ObjectDisposedException(window.GetType().Name, "The window closed before JavaScript acknowledged the message.");
            await completion.Task.ConfigureAwait(false);
            finalState = "Acknowledged";
        }
        catch (OperationCanceledException exception) {
            finalState = "Cancelled";
            failureReason = exception.Message;
            throw;
        }
        catch (Exception exception) {
            failureReason = exception.Message;
            throw;
        }
        finally {
            _acknowledgements.TryRemove(id, out _);
            (window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
                diagnosticKey, finalState, failureReason: failureReason
            );
        }
    }

    private static string CreateAcknowledgementPayload(ulong id, string message) {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) {
            writer.WriteStartObject();
            writer.WriteString("OperationId", id.ToString());
            writer.WriteString("Message", message);
            writer.WriteEndObject();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}