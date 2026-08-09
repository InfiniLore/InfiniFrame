// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class JavaScriptInfiniFrameWindowFeature : IJavaScriptInfiniFrameWindowFeature {
    private static long _nextRequestId;
    private readonly IInfiniFrameWindow window;
    private readonly ILogger<JavaScriptInfiniFrameWindowFeature> logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string?>> _pendingEvals = new();

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

            if (!_pendingEvals.TryRemove(requestId, out TaskCompletionSource<string?>? completion))
                return;

            if (root.TryGetProperty("error", out JsonElement errorElement)
                && errorElement.ValueKind == JsonValueKind.String) {
                completion.TrySetException(new JavaScriptEvaluationException(errorElement.GetString() ?? "JavaScript evaluation failed."));
                return;
            }

            string? result = null;
            if (root.TryGetProperty("result", out JsonElement resultElement)
                && resultElement.ValueKind != JsonValueKind.Null) {
                result = resultElement.GetRawText();
            }

            completion.TrySetResult(result);
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
}

/// <summary>
///     Represents an error that occurred during JavaScript evaluation in the browser control.
/// </summary>
public sealed class JavaScriptEvaluationException(string message) : Exception(message);
