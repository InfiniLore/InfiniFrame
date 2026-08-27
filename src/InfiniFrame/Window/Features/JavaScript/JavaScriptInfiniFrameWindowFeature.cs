// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature implementation for executing JavaScript in the native WebView, supporting both fire-and-forget
///     evaluation and request/response patterns with result deserialization.
/// </summary>
public class JavaScriptInfiniFrameWindowFeature : IJavaScriptInfiniFrameWindowFeature {
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string?>> _pendingEvals = new();
    private readonly ILogger<JavaScriptInfiniFrameWindowFeature> logger;
    private readonly IInfiniFrameWindow window;
    private long _nextRequestId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JavaScriptInfiniFrameWindowFeature"/> class.
    /// </summary>
    /// <param name="window">The window instance to execute JavaScript in.</param>
    /// <param name="logger">The logger instance.</param>
    public JavaScriptInfiniFrameWindowFeature(
        IInfiniFrameWindow window,
        ILogger<JavaScriptInfiniFrameWindowFeature> logger
    ) {
        this.window = window;
        this.logger = logger;
        window.EventsStore.WebMessagePostData.Add(
            JsHandlerNames.JavaScriptEvalResult,
            HandleEvalResult
        );
    }

    /// <inheritdoc cref="IJavaScriptInfiniFrameWindowFeature.ExecuteJavaScriptAsync" />
    public ValueTask<string?> ExecuteJavaScriptAsync(string script, CancellationToken ct = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(script);
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled<string?>(ct);

        return !window.IsClosedOrClosing()
            ? ExecuteLocallyAsync(script, ct)
            : ValueTask.FromException<string?>(new ObjectDisposedException(window.GetType().Name));

    }

    /// <inheritdoc cref="IJavaScriptInfiniFrameWindowFeature.ExecuteJavaScriptAsync{T}" />
    public async ValueTask<T?> ExecuteJavaScriptAsync<T>(string script, CancellationToken ct = default) {
        string? json = await ExecuteJavaScriptAsync(script, ct).ConfigureAwait(false);
        if (json is null) return default;

        return JsonSerializer.Deserialize(json, WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(typeof(T))!) is T result
            ? result
            : default;
    }

    /// <inheritdoc cref="IJavaScriptInfiniFrameWindowFeature.SendEvalToBrowser" />
    public void SendEvalToBrowser(string script, string? requestId = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(script);
        if (window.IsClosedOrClosing()) return;

        string evalRequestId = requestId ?? $"eval_{unchecked((ulong)Interlocked.Increment(ref _nextRequestId))}";
        string envelope = CreateEvalRequestEnvelope(evalRequestId, script);
        window.Features.WebMessaging.SendWebMessage(envelope);
    }

    private async ValueTask<string?> ExecuteLocallyAsync(string script, CancellationToken ct) {
        string requestId = $"eval_{unchecked((ulong)Interlocked.Increment(ref _nextRequestId))}";
        string? diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation("ExecuteJavaScript", unchecked((ulong)_nextRequestId));
        string finalState = "Failed";
        string? failureReason = null;

        var completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_pendingEvals.TryAdd(requestId, completion))
            throw new InvalidOperationException("Could not register the JavaScript evaluation request.");

        try {
            string envelope = CreateEvalRequestEnvelope(requestId, script);
            await window.Features.WebMessaging.SendWebMessageAsync(envelope, ct).ConfigureAwait(false);

            Task closed = window.WaitForCloseAsync().AsTask();
            Task terminal = await Task.WhenAny(completion.Task, closed).WaitAsync(ct).ConfigureAwait(false);
            if (terminal == closed)
                throw new ObjectDisposedException(window.GetType().Name, "The window closed before JavaScript evaluation completed.");

            string? result = await completion.Task.ConfigureAwait(false);
            finalState = "Succeeded";
            return result;
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
            _pendingEvals.TryRemove(requestId, out _);
            (window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
                diagnosticKey, finalState, failureReason: failureReason
            );
        }
    }

    private void HandleEvalResult(IInfiniFrameWindow sender, string? payload) {
        if (string.IsNullOrWhiteSpace(payload)) return;

        try {
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return;

            if (!root.TryGetProperty("requestId", out JsonElement requestIdElement)
                || requestIdElement.ValueKind != JsonValueKind.String)
                return;

            string? requestId = requestIdElement.GetString();
            if (requestId is null) return;

            string? error = null;
            if (root.TryGetProperty("error", out JsonElement errorElement)
                && errorElement.ValueKind == JsonValueKind.String) {
                error = errorElement.GetString();
            }

            string? result = null;
            if (root.TryGetProperty("result", out JsonElement resultElement)
                && resultElement.ValueKind != JsonValueKind.Null) {
                result = resultElement.GetRawText();
            }

            if (_pendingEvals.TryRemove(requestId, out TaskCompletionSource<string?>? completion)) {
                if (error is not null) {
                    completion.TrySetException(new JavaScriptEvaluationException(error));
                }
                else {
                    completion.TrySetResult(result);
                }

                return;
            }

            string responsePayload = CreateEvalResponsePayload(requestId, result, error);
            string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(
                JsHandlerNames.JavaScriptEvalResponse,
                responsePayload
            );
            sender.Features.WebMessaging.SendWebMessage(responseEnvelope);
        }
        catch (JsonException exception) {
            logger.LogWarning(exception, "Failed to parse JavaScript evaluation result.");
        }
    }

    private static string CreateEvalRequestEnvelope(string requestId, string script) {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) {
            writer.WriteStartObject();
            writer.WriteString("requestId", requestId);
            writer.WriteString("script", script);
            writer.WriteEndObject();
        }

        return InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.JavaScriptEvalRequest,
            Encoding.UTF8.GetString(stream.ToArray())
        );
    }

    private static string CreateEvalResponsePayload(string requestId, string? result, string? error) {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream)) {
            writer.WriteStartObject();
            writer.WriteString("requestId", requestId);
            if (result is not null) {
                writer.WritePropertyName("result");
                writer.WriteRawValue(result);
            }
            else {
                writer.WriteNull("result");
            }

            if (error is not null) writer.WriteString("error", error);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
