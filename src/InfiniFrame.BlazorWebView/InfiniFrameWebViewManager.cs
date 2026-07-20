// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;
using InfiniFrame.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebViewManager : WebViewManager, IInfiniFrameWebViewManager {

    // BlazorWebView resources are always hosted on a dedicated app:// origin.
    // This keeps module/script fetches on the same trusted internal origin
    // across platforms and avoids localhost/CORS routing edge-cases.
    public const string BlazorAppScheme = "app";
    public const string AppBaseUri = $"{BlazorAppScheme}://localhost/";

    private readonly Channel<string> _channel;
    private readonly CancellationTokenSource _messagePumpShutdown = new();
    private readonly int _messageQueueCapacity;
    private readonly BoundedChannelFullMode _messageQueueFullMode;
    private int _disposeStarted;
    private int _disposed;

    private readonly Task _messagePumpTask;
    private readonly IInfiniFrameUriSecurityPolicy _uriSecurityPolicy;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameWebViewManager(
        IInfiniFrameWindowBuilder builder,
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        IOptions<InfiniFrameBlazorAppConfiguration> config
    )
        : base(provider, dispatcher, config.Value.AppBaseUri, fileProvider, jsComponents, config.Value.HostPage) {
        InfiniFrameBlazorAppConfiguration configuration = config.Value;
        if (configuration.WebMessageQueueCapacity <= 0) {
            throw new ArgumentOutOfRangeException(
                nameof(configuration.WebMessageQueueCapacity),
                configuration.WebMessageQueueCapacity,
                "The WebView message queue capacity must be positive.");
        }

        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(configuration.WebMessageQueueCapacity) {
            SingleReader = true,
            SingleWriter = false,
            FullMode = configuration.WebMessageQueueFullMode,
            AllowSynchronousContinuations = false
        });
        _messageQueueCapacity = configuration.WebMessageQueueCapacity;
        _messageQueueFullMode = configuration.WebMessageQueueFullMode;
        _uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry
            .GetForBuilder(builder)
            .WithTrustedOrigin(configuration.AppBaseUri);

        // ReSharper disable once ConvertClosureToMethodGroup
        LazyWindow = new Lazy<IInfiniFrameWindow>(() => provider.GetRequiredService<IInfiniFrameWindow>());
        // ReSharper disable once ConvertClosureToMethodGroup
        LazyLogger = new Lazy<ILogger<InfiniFrameWebViewManager>?>(() => provider.GetService<ILogger<InfiniFrameWebViewManager>>());

        builder.RegisterWebMessageReceivedHandler((_, message, origin) => {
            if (IsDisposingOrDisposed) return;

            LazyLogger.Value?.LogTrace("Web message callback received from native. Origin: {Origin}, Length: {Length}", origin, message.Length);

            try {
                HandleWebMessage((message, origin));
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                LazyLogger.Value?.LogWarning(ex, "Unhandled exception while handling native web message callback.");
            }
        });

        _messagePumpTask = MessagePump();
        LazyLogger.Value?.LogDebug(
            "Started WebView message pump. QueueCapacity: {QueueCapacity}, FullMode: {FullMode}",
            configuration.WebMessageQueueCapacity,
            configuration.WebMessageQueueFullMode);
    }

    private Lazy<IInfiniFrameWindow> LazyWindow { get; }
    private Lazy<ILogger<InfiniFrameWebViewManager>?> LazyLogger { get; }
    private bool IsDisposingOrDisposed => Volatile.Read(ref _disposeStarted) != 0 || Volatile.Read(ref _disposed) != 0;

    // -----------------------------------------------------------------------------------------------------------------
    // Web Requests
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWebViewManager.HandleWebRequest" />
    public (Stream? Data, string? ContentType) HandleWebRequest(IInfiniFrameWindow? infiniFrameWindow, string? url) {
        if (string.IsNullOrWhiteSpace(url)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request because URL is null or empty. Url: {Url}",
                url
            );
            return default;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? requestUri)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request because URL parsing failed. Url: {Url}",
                url
            );
            return default;
        }

        // ---------------------------------------------------------------------
        // IMPORTANT FIX: allow Blazor + app internal scheme BEFORE validation
        // ---------------------------------------------------------------------
        if (requestUri.Scheme == BlazorAppScheme) {
            // no security policy, no warnings, this is framework/app internal traffic

            string localPath = requestUri.LocalPath;
            bool hasFileExtension = Path.HasExtension(localPath);

            Uri sanitizedUri = new UriBuilder(requestUri) {
                Query = string.Empty,
                Fragment = string.Empty
            }.Uri;

            if (!TryGetResponseContent(sanitizedUri.AbsoluteUri, !hasFileExtension, out _, out _, out Stream content, out IDictionary<string, string> headers))
                return default;

            headers.TryGetValue("Content-Type", out string? contentType);
            return (content, contentType ?? GetFallbackContentType(sanitizedUri.LocalPath));
        }

        // ---------------------------------------------------------------------
        // External / non-Blazor traffic (secure path)
        // ---------------------------------------------------------------------
        if (!_uriSecurityPolicy.IsNavigationSchemeAllowed(requestUri.Scheme)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request due to disallowed URI scheme. Scheme: {Scheme}, Url: {Url}",
                requestUri.Scheme,
                requestUri);
            return default;
        }

        if (!_uriSecurityPolicy.IsTrustedOrigin(requestUri)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request due to untrusted origin. RequestOrigin: {RequestOrigin}, TrustedOrigins: {TrustedOrigins}",
                requestUri,
                _uriSecurityPolicy.TrustedOrigins);
            return default;
        }

        string localPath2 = requestUri.LocalPath;
        bool hasFileExtension2 = Path.HasExtension(localPath2);

        Uri sanitizedUri2 = new UriBuilder(requestUri) {
            Query = string.Empty,
            Fragment = string.Empty
        }.Uri;

        if (TryGetResponseContent(
            sanitizedUri2.AbsoluteUri,
            !hasFileExtension2,
            out _,
            out _,
            out Stream content2,
            out IDictionary<string, string> headers2)
        ) {
            headers2.TryGetValue("Content-Type", out string? contentType);
            return (content2, contentType ?? GetFallbackContentType(sanitizedUri2.LocalPath));
        }

        LazyLogger.Value?.LogWarning(
            "No web content found for trusted URL. Url: {Url}",
            sanitizedUri2);

        return default;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Web message handling
    // -----------------------------------------------------------------------------------------------------------------
    private void HandleWebMessage((string Message, string? Origin) state) {
        if (IsDisposingOrDisposed) return;

        Uri? messageOriginUrl;

        if (!string.IsNullOrWhiteSpace(state.Origin)) {
            if (!Uri.TryCreate(state.Origin, UriKind.Absolute, out messageOriginUrl)) {
                LazyLogger.Value?.LogWarning(
                    "Rejected web message because origin parsing failed. Origin: {Origin}",
                    state.Origin);
                return;
            }
        }
        else if (Uri.TryCreate(AppBaseUri, UriKind.Absolute, out Uri? fallback)) {
            messageOriginUrl = fallback;

            LazyLogger.Value?.LogDebug(
                "Web message origin missing. Falling back to AppBaseUri origin: {FallbackOrigin}",
                fallback);
        }
        else {
            LazyLogger.Value?.LogWarning(
                "Rejected web message because origin is missing or unknown.");
            return;
        }

        if (!_uriSecurityPolicy.IsTrustedOrigin(messageOriginUrl)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web message due to origin mismatch. Origin: {MessageOrigin}, TrustedOrigins: {TrustedOrigins}",
                messageOriginUrl,
                _uriSecurityPolicy.TrustedOrigins);
            return;
        }

        // The callback runs on the native UI thread. Do not hold a lifecycle lock while dispatching
        // messages because the pump synchronously invokes that same thread to send responses.
        if (IsDisposingOrDisposed) return;

        MessageReceived(messageOriginUrl, state.Message);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------
    protected override void NavigateCore(Uri absoluteUri) {
        LazyWindow.Value.Features.PageNavigation.Load(absoluteUri);
    }

    protected override void SendMessage(string message) {
        if (IsDisposingOrDisposed || _messagePumpShutdown.IsCancellationRequested) {
            LazyLogger.Value?.LogTrace("Discarded outbound WebView message because the manager is shutting down.");
            return;
        }

        if (_channel.Writer.TryWrite(message)) return;

        LazyLogger.Value?.LogWarning(
            "Discarded outbound WebView message because the bounded queue is unavailable or full. QueueCapacity: {QueueCapacity}, FullMode: {FullMode}",
            _messageQueueCapacity,
            _messageQueueFullMode);
    }

    private async Task MessagePump() {
        try {
            while (await _channel.Reader.WaitToReadAsync(_messagePumpShutdown.Token)) {
                while (_channel.Reader.TryRead(out string? message)) {
                    if (IsDisposingOrDisposed || _messagePumpShutdown.IsCancellationRequested) return;

                    await LazyWindow.Value.SendWebMessageAsync(message, _messagePumpShutdown.Token).ConfigureAwait(false);
                }
            }
        }
        catch (ObjectDisposedException ex) {
            LazyLogger.Value?.LogDebug(ex, "WebView message pump observed disposed dependencies; stopping.");
        }
        catch (ChannelClosedException ex) {
            LazyLogger.Value?.LogDebug(ex, "WebView message channel closed; stopping message pump.");
        }
        catch (OperationCanceledException) {
            LazyLogger.Value?.LogDebug("WebView message pump cancellation requested.");
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            LazyLogger.Value?.LogError(ex, "Unhandled exception in WebView message pump.");
        }
    }

    protected override async ValueTask DisposeAsyncCore() {
        if (Interlocked.Exchange(ref _disposeStarted, 1) != 0) return;

        _channel.Writer.TryComplete();
        _messagePumpShutdown.Cancel();

        try {
            // Some tests build and dispose of the app without ever creating a native window.
            // In that case, disposing the base WebView manager can flow through dispatch paths
            // that resolve IInfiniFrameWindow and initialize native resources during teardown.
            // Avoid creating a window while disposing of an app that never ran.
            if (LazyWindow.IsValueCreated) {
                await base.DisposeAsyncCore();
            }
        }
        finally {
            try {
                await _messagePumpTask.ConfigureAwait(false);
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                LazyLogger.Value?.LogWarning(ex, "Message pump faulted during WebView manager shutdown.");
            }
            finally {
                _messagePumpShutdown.Dispose();
                Volatile.Write(ref _disposed, 1);
                LazyLogger.Value?.LogDebug("WebView manager disposal completed after the message pump stopped.");
            }
        }
    }

    private static string GetFallbackContentType(string localPath) {
        string extension = Path.GetExtension(localPath);

        if (string.IsNullOrWhiteSpace(extension)) return "application/octet-stream";

        return extension.ToLowerInvariant() switch {
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".js" or ".mjs" => "text/javascript; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".wasm" => "application/wasm",
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".ico" => "image/x-icon",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            _ => "application/octet-stream"
        };
    }
}
