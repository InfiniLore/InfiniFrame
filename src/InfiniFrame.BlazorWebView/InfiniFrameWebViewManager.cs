// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Channels;
using InfiniFrame.Security;
using InfiniFrame.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Manages the Blazor web view lifecycle, resource serving, and custom URL scheme handling.
/// </summary>
public class InfiniFrameWebViewManager : WebViewManager, IInfiniFrameWebViewManager {

    // BlazorWebView resources are always hosted on a dedicated app:// origin.
    // This keeps module/script fetches on the same trusted internal origin
    // across platforms and avoids localhost/CORS routing edge-cases.
    /// <summary>
    ///     The custom URL scheme used for serving Blazor application resources.
    /// </summary>
    public const string BlazorAppScheme = "app";
    /// <summary>
    ///     The base URI for the Blazor application's internal origin.
    /// </summary>
    public const string AppBaseUri = $"{BlazorAppScheme}://localhost/";

    private readonly Channel<string> _channel;
    private readonly CancellationTokenSource _messagePumpShutdown = new();
    private readonly ILogger<InfiniFrameWebViewManager> _logger;

    private readonly Task _messagePumpTask;
    private readonly int _messageQueueCapacity;
    private readonly BoundedChannelFullMode _messageQueueFullMode;
    private readonly IInfiniFrameUriSecurityPolicy _fallbackUriSecurityPolicy;
    private int _disposeStarted;
    private int _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructor
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Initializes a new instance of the <see cref="InfiniFrameWebViewManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency injection.</param>
    /// <param name="dispatcher">The Blazor dispatcher for thread marshalling.</param>
    /// <param name="fileProvider">The file provider for serving static assets.</param>
    /// <param name="jsComponents">The JavaScript component configuration store.</param>
    /// <param name="config">The Blazor application configuration.</param>
    /// <param name="logger">The logger</param>
    public InfiniFrameWebViewManager(
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        IOptions<InfiniFrameBlazorAppConfiguration> config,
        ILogger<InfiniFrameWebViewManager> logger
    )
        : base(provider, dispatcher, config.Value.AppBaseUri, fileProvider, jsComponents, config.Value.HostPage) {
        _logger = logger;
        InfiniFrameBlazorAppConfiguration configuration = config.Value;
        if (configuration.WebMessageQueueCapacity <= 0) {
            throw new ArgumentOutOfRangeException(
                nameof(configuration.WebMessageQueueCapacity),
                configuration.WebMessageQueueCapacity,
                "The WebView message queue capacity must be positive.");
        }

        // TryWrite cannot report which item DropWrite discarded. Use Wait so the non-awaitable
        // producer receives a false result and the loss is observable through diagnostics.
        BoundedChannelFullMode effectiveFullMode = configuration.WebMessageQueueFullMode == BoundedChannelFullMode.DropWrite
            ? BoundedChannelFullMode.Wait
            : configuration.WebMessageQueueFullMode;
        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(configuration.WebMessageQueueCapacity) {
            SingleReader = true,
            SingleWriter = false,
            FullMode = effectiveFullMode,
            AllowSynchronousContinuations = false
        });
        _messageQueueCapacity = configuration.WebMessageQueueCapacity;
        _messageQueueFullMode = effectiveFullMode;
        _fallbackUriSecurityPolicy = InfiniFrameUriSecurityPolicy.Default
            .WithTrustedOrigin(configuration.AppBaseUri);

        // ReSharper disable once ConvertClosureToMethodGroup
        LazyWindow = new Lazy<IInfiniFrameWindow>(() => provider.GetRequiredService<IInfiniFrameWindow>());

        _messagePumpTask = MessagePump();
        _logger.LogDebug(
            "Started WebView message pump. QueueCapacity: {QueueCapacity}, FullMode: {FullMode}",
            configuration.WebMessageQueueCapacity,
            configuration.WebMessageQueueFullMode);
    }

    private Lazy<IInfiniFrameWindow> LazyWindow { get; }
    private bool IsDisposingOrDisposed => Volatile.Read(ref _disposeStarted) != 0 || Volatile.Read(ref _disposed) != 0;

    // -----------------------------------------------------------------------------------------------------------------
    // Web Requests
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWebViewManager.HandleWebRequest" />
    public (Stream? Data, string? ContentType) HandleWebRequest(IInfiniFrameWindow? infiniFrameWindow, string? url) {
        if (string.IsNullOrWhiteSpace(url)) {
            _logger.LogWarning(
                "Rejected web request because URL is null or empty. Url: {Url}",
                url
            );
            return default;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? requestUri)) {
            _logger.LogWarning(
                "Rejected web request because URL parsing failed. Url: {Url}",
                url
            );
            return default;
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = GetUriSecurityPolicy(infiniFrameWindow);
        if (!uriSecurityPolicy.IsNavigationSchemeAllowed(requestUri.Scheme)) {
            _logger.LogWarning(
                "Rejected web request due to disallowed URI scheme. Scheme: {Scheme}, Url: {Url}",
                requestUri.Scheme,
                requestUri);
            return default;
        }

        if (!uriSecurityPolicy.IsTrustedOrigin(requestUri)) {
            _logger.LogWarning(
                "Rejected web request due to untrusted origin. RequestOrigin: {RequestOrigin}, TrustedOrigins: {TrustedOrigins}",
                requestUri,
                uriSecurityPolicy.TrustedOrigins);
            return default;
        }

        string localPath = requestUri.LocalPath;
        bool hasFileExtension = Path.HasExtension(localPath);

        // Query strings and fragments identify browser state, not embedded files. Keep the original
        // navigation URI in the WebView and remove these components only for resource lookup.
        Uri resourceUri = new UriBuilder(requestUri) {
            Query = string.Empty,
            Fragment = string.Empty
        }.Uri;

        if (TryGetResponseContent(
            resourceUri.AbsoluteUri,
            !hasFileExtension,
            out _,
            out _,
            out Stream content2,
            out IDictionary<string, string> headers2)
        ) {
            headers2.TryGetValue("Content-Type", out string? contentType);
            return (content2, contentType ?? GetFallbackContentType(resourceUri.LocalPath));
        }

        _logger.LogWarning(
            "No web content found for trusted URL. Url: {Url}",
            resourceUri);

        return default;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Web message handling
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWebViewManager.HandleWebMessage" />
    public void HandleWebMessage(IInfiniFrameWindow window, string message, string? origin) {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(message);

        if (IsDisposingOrDisposed) return;

        _logger.LogTrace("Web message callback received from native. Origin: {Origin}, Length: {Length}", origin, message.Length);

        try {
            HandleWebMessageCore(window, message, origin);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            _logger.LogWarning(ex, "Unhandled exception while handling native web message callback.");
        }
    }

    private void HandleWebMessageCore(IInfiniFrameWindow window, string message, string? origin) {
        if (IsDisposingOrDisposed) return;

        Uri? messageOriginUrl;

        if (!string.IsNullOrWhiteSpace(origin)) {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out messageOriginUrl)) {
                _logger.LogWarning(
                    "Rejected web message because origin parsing failed. Origin: {Origin}",
                    origin);
                return;
            }
        }
        else if (Uri.TryCreate(AppBaseUri, UriKind.Absolute, out Uri? fallback)) {
            messageOriginUrl = fallback;

            _logger.LogDebug(
                "Web message origin missing. Falling back to AppBaseUri origin: {FallbackOrigin}",
                fallback);
        }
        else {
            _logger.LogWarning(
                "Rejected web message because origin is missing or unknown.");
            return;
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = GetUriSecurityPolicy(window);
        if (!uriSecurityPolicy.IsTrustedOrigin(messageOriginUrl)) {
            _logger.LogWarning(
                "Rejected web message due to origin mismatch. Origin: {MessageOrigin}, TrustedOrigins: {TrustedOrigins}",
                messageOriginUrl,
                uriSecurityPolicy.TrustedOrigins);
            return;
        }

        // The callback runs on the native UI thread. Do not hold a lifecycle lock while dispatching
        // messages because the pump synchronously invokes that same thread to send responses.
        if (IsDisposingOrDisposed) return;

        MessageReceived(messageOriginUrl, message);
    }

    private IInfiniFrameUriSecurityPolicy GetUriSecurityPolicy(IInfiniFrameWindow? window) =>
        window is null
            ? _fallbackUriSecurityPolicy
            : InfiniFrameUriSecurityPolicyRegistry.GetForWindow(window);

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------
    protected override void NavigateCore(Uri absoluteUri) {
        LazyWindow.Value.Features.PageNavigation.Load(absoluteUri);
    }

    protected override void SendMessage(string message) {
        if (IsDisposingOrDisposed || _messagePumpShutdown.IsCancellationRequested) {
            _logger.LogTrace("Discarded outbound WebView message because the manager is shutting down.");
            return;
        }

        if (_channel.Writer.TryWrite(message)) return;

        _logger.LogError(
            "Discarded outbound WebView message because the bounded queue is unavailable or full. " +
            "This may cause stale UI state. QueueCapacity: {QueueCapacity}, FullMode: {FullMode}",
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
            _logger.LogDebug(ex, "WebView message pump observed disposed dependencies; stopping.");
        }
        catch (ChannelClosedException ex) {
            _logger.LogDebug(ex, "WebView message channel closed; stopping message pump.");
        }
        catch (OperationCanceledException) {
            _logger.LogDebug("WebView message pump cancellation requested.");
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            _logger.LogError(ex, "Unhandled exception in WebView message pump.");
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
                _logger.LogWarning(ex, "Message pump faulted during WebView manager shutdown.");
            }
            finally {
                _messagePumpShutdown.Dispose();
                Volatile.Write(ref _disposed, 1);
                _logger.LogDebug("WebView manager disposal completed after the message pump stopped.");
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
