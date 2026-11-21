// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text;
using InfiniFrame.Hosting;
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <remarks>
///     Resource dispatch reuses the window's registered custom-scheme handlers (for Blazor, the WebView manager). A
///     failure is a status for that request only and never faults the window or host: the handler owner's own status
///     when it reports one (the Blazor manager: for example 404 with its body, or 403 for an untrusted origin), otherwise
///     200 for a body and 404 for none; 405 for other methods; 500 for a handler failure or an invalid content type;
///     503 once the window has ended. The handler's stream is passed through unbuffered; ownership moves to the provider.
/// </remarks>
internal sealed partial class InfiniFrameHostedWindow {
    private const string DefaultContentType = "application/octet-stream";

    private async ValueTask<InfiniFrameHostResourceResponse> OnHostGetResourceAsync(
        InfiniFrameHostResourceRequest request,
        CancellationToken cancellationToken
    ) {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        if (LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return Status(503);
        if (request.Url is not { IsAbsoluteUri: true } url) return Status(404);
        bool head = string.Equals(request.Method, "HEAD", StringComparison.OrdinalIgnoreCase);
        if (!head && !string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase)) return Status(405);
        if (!EventsStore.CustomScheme.Snapshot.TryGetValue(url.Scheme.ToLowerInvariant(),
                out Func<IInfiniFrameWindow, string, (Stream? Data, string? ContentType)>? handler))
            return Status(404);

        (int StatusCode, Stream? Body, string? ContentType) result;
        try {
            // Handlers are synchronous. Run them off the provider's thread so a slow asset never blocks its loop.
            // A core handler owner that knows the real status (the Blazor WebView manager) is asked for it directly;
            // any other handler keeps the public contract: a body is 200, no body is 404.
            result = await Task.Run(() => handler.Target is IInfiniFrameStatusResourceSource statusSource
                ? statusSource.GetResource(this, url.OriginalString)
                : Untyped(handler(this, url.OriginalString)), cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            throw;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Custom scheme handler failed for {Url}. Window: {WindowId}", url, Id);
            return Status(500);
        }
        if (result.Body is not {} body) return Status(result.StatusCode is >= 400 and <= 599 ? result.StatusCode : 404);

        bool transferred = false;
        try {
            cancellationToken.ThrowIfCancellationRequested();
            string contentType = string.IsNullOrWhiteSpace(result.ContentType) ? DefaultContentType : result.ContentType;
            if (!IsValidContentType(contentType) || result.StatusCode is < 100 or > 599) {
                _logger.LogError("Custom scheme handler returned an invalid content type or status for {Url}. Window: {WindowId}", url, Id);
                return Status(500);
            }
            long? length = TryGetRemainingLength(body);
            if (head) return new InfiniFrameHostResourceResponse { StatusCode = result.StatusCode, ContentType = contentType, ContentLength = length };
            transferred = true;
            return new InfiniFrameHostResourceResponse { StatusCode = result.StatusCode, ContentType = contentType, ContentLength = length, Body = body };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            throw;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Preparing the response for {Url} failed. Window: {WindowId}", url, Id);
            return Status(500);
        }
        finally {
            if (!transferred) {
                try { await body.DisposeAsync().ConfigureAwait(false); }
                catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                    _logger.LogDebug(exception, "Disposing an unused response body failed. Window: {WindowId}", Id);
                }
            }
        }
    }

    private static InfiniFrameHostResourceResponse Status(int statusCode) => new() { StatusCode = statusCode };

    private static (int StatusCode, Stream? Body, string? ContentType) Untyped((Stream? Data, string? ContentType) result) =>
        (result.Data is null ? 404 : 200, result.Data, result.ContentType);

    // Same rules the default engine applies before handing a content type to native code.
    private static bool IsValidContentType(string contentType) =>
        contentType.IndexOfAny(['\r', '\n', '\0', '\t']) < 0
        && Encoding.UTF8.GetByteCount(contentType) <= CustomSchemeResponse.MaxContentTypeBytes;

    private static long? TryGetRemainingLength(Stream stream) {
        try {
            return stream.CanSeek ? checked(stream.Length - stream.Position) : null;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            return null;
        }
    }
}
