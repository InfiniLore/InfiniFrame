// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Channels;
using InfiniFrame.Hosting;
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
public class InfiniFrameWebViewManager : WebViewManager, IInfiniFrameWebViewManager, IInfiniFrameStatusResourceSource {

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

    private readonly Channel<QueuedWebMessage> _channel;
    private readonly CancellationTokenSource _messagePumpShutdown = new();

    // Hosted windows only (external host providers). Blazor keeps one page context per AttachPage, and everything it
    // sends belongs to that page. Each outbound message is therefore bound, when produced, to the browser document whose
    // AttachPage created the current page context (recorded on Blazor's dispatcher, after work already queued for the
    // previous page). Ambient execution context is deliberately not used: long-lived work started under one page (for
    // example a singleton's timer) can later drive the next page. Work for a retired document is cancelled, never sent
    // to a later document, and an over-full queue faults the window instead of dropping messages.
    private const string AttachPageMessagePrefix = "__bwv:[\"AttachPage\"";
    private const long HostedQueueByteCapacity = 64L * 1024 * 1024;
    private readonly Dispatcher _blazorDispatcher;
    private IInfiniFrameDocumentWindow? _documentWindow;
    private ulong _pageDocument;
    private long _hostedQueuedBytes;
    private readonly ILogger<InfiniFrameWebViewManager> _logger;

    private readonly Task _messagePumpTask;
    private readonly int _messageQueueCapacity;
    private readonly BoundedChannelFullMode _messageQueueFullMode;
    private readonly Uri _appBaseUri;
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
    /// <param name="dispatcher">The Blazor dispatcher for thread marshaling.</param>
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
        _blazorDispatcher = dispatcher;
        InfiniFrameBlazorAppConfiguration configuration = config.Value;
        if (configuration.AppBaseUri is null
            || !configuration.AppBaseUri.IsAbsoluteUri
            || string.IsNullOrWhiteSpace(configuration.AppBaseUri.Scheme)
            || string.IsNullOrWhiteSpace(configuration.AppBaseUri.Host)) {
            throw new ArgumentException("AppBaseUri must be an absolute URI with a scheme and host.", nameof(configuration.AppBaseUri));
        }

        if (!string.Equals(configuration.AppBaseUri.Scheme, BlazorAppScheme, StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException(
                $"AppBaseUri must use the '{BlazorAppScheme}' scheme. Got '{configuration.AppBaseUri.Scheme}'.",
                nameof(configuration.AppBaseUri));
        }
        _appBaseUri = configuration.AppBaseUri;
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
        _channel = Channel.CreateBounded<QueuedWebMessage>(new BoundedChannelOptions(configuration.WebMessageQueueCapacity) {
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
        (_, Stream? data, string? contentType) = GetResourceCore(infiniFrameWindow, url);
        return (data, contentType);
    }

    // Hosted windows read the status that the public tuple contract above cannot carry (for example, the 404 response
    // the Blazor static content provider returns for a missing asset). Default engines keep using HandleWebRequest.
    (int StatusCode, Stream? Body, string? ContentType) IInfiniFrameStatusResourceSource.GetResource(IInfiniFrameWindow? window, string? url)
        => GetResourceCore(window, url);

    private (int StatusCode, Stream? Body, string? ContentType) GetResourceCore(IInfiniFrameWindow? infiniFrameWindow, string? url) {
        if (string.IsNullOrWhiteSpace(url)) {
            _logger.LogWarning(
                "Rejected web request because URL is null or empty. Url: {Url}",
                url
            );
            return (404, null, null);
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? requestUri)) {
            _logger.LogWarning(
                "Rejected web request because URL parsing failed. Url: {Url}",
                url
            );
            return (404, null, null);
        }

        IInfiniFrameUriSecurityPolicy uriSecurityPolicy = GetUriSecurityPolicy(infiniFrameWindow);
        if (!uriSecurityPolicy.IsNavigationSchemeAllowed(requestUri.Scheme)) {
            _logger.LogWarning(
                "Rejected web request due to disallowed URI scheme. Scheme: {Scheme}, Url: {Url}",
                requestUri.Scheme,
                requestUri);
            return (403, null, null);
        }

        if (!uriSecurityPolicy.IsTrustedOrigin(requestUri)) {
            _logger.LogWarning(
                "Rejected web request due to untrusted origin. RequestOrigin: {RequestOrigin}, TrustedOrigins: {TrustedOrigins}",
                requestUri,
                uriSecurityPolicy.TrustedOrigins);
            return (403, null, null);
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
            out int statusCode,
            out _,
            out Stream content2,
            out IDictionary<string, string> headers2)
        ) {
            headers2.TryGetValue("Content-Type", out string? contentType);
            return (statusCode == 0 ? 200 : statusCode, content2, contentType ?? GetFallbackContentType(resourceUri.LocalPath));
        }

        _logger.LogWarning(
            "No web content found for trusted URL. Url: {Url}",
            resourceUri);

        return (404, null, null);
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
            _logger.LogWarning(ex, "Unhandled exception while handling native web message callback");
        }
    }

    private void HandleWebMessageCore(IInfiniFrameWindow window, string message, string? origin) {
        if (IsDisposingOrDisposed) return;

        IInfiniFrameDocumentWindow? documentWindow = window as IInfiniFrameDocumentWindow;
        Uri? messageOriginUrl;

        if (!string.IsNullOrWhiteSpace(origin)) {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out messageOriginUrl)) {
                _logger.LogWarning(
                    "Rejected web message because origin parsing failed. Origin: {Origin}",
                    origin);
                return;
            }
        }
        // A hosted window always supplies the browser-reported document origin, so it never falls back.
        else if (documentWindow is null && _appBaseUri.IsAbsoluteUri) {
            messageOriginUrl = _appBaseUri;

            _logger.LogDebug(
                "Web message origin missing. Falling back to AppBaseUri origin: {FallbackOrigin}",
                _appBaseUri);
        }
        else {
            _logger.LogWarning(
                "Rejected web message because origin is missing or unknown");
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

        if (documentWindow is null) {
            MessageReceived(messageOriginUrl, message);
            return;
        }

        ulong? incoming = documentWindow.IncomingMessageDocument;
        if (incoming is null or 0 || incoming.Value != documentWindow.CurrentDocument) {
            _logger.LogWarning("Rejected a web message from a superseded or unknown hosted document");
            return;
        }
        Volatile.Write(ref _documentWindow, documentWindow);
        if (!message.StartsWith(AttachPageMessagePrefix, StringComparison.Ordinal)) {
            MessageReceived(messageOriginUrl, message);
            return;
        }
        // Record the attaching document on Blazor's dispatcher, in order with the work already queued there, so output
        // still pending for the previous page context stays bound to the previous document.
        ulong document = incoming.Value;
        Uri attachOrigin = messageOriginUrl;
        _ = ObserveHostedAttachAsync(_blazorDispatcher.InvokeAsync(() => {
            Volatile.Write(ref _pageDocument, document);
            MessageReceived(attachOrigin, message);
        }));
    }

    private async Task ObserveHostedAttachAsync(Task attach) {
        try {
            await attach.ConfigureAwait(false);
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            _logger.LogWarning(ex, "Attaching a hosted Blazor page failed");
        }
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
            _logger.LogTrace("Discarded outbound WebView message because the manager is shutting down");
            return;
        }

        if (Volatile.Read(ref _documentWindow) is {} documentWindow) {
            QueueHostedMessage(documentWindow, message);
            return;
        }

        if (_channel.Writer.TryWrite(new QueuedWebMessage(message, 0, false))) return;

        _logger.LogError(
            "Discarded outbound WebView message because the bounded queue is unavailable or full. " +
            "This may cause stale UI state. QueueCapacity: {QueueCapacity}, FullMode: {FullMode}",
            _messageQueueCapacity,
            _messageQueueFullMode);
    }

    private void QueueHostedMessage(IInfiniFrameDocumentWindow documentWindow, string message) {
        ulong document = Volatile.Read(ref _pageDocument);
        long bytes = (long)message.Length * sizeof(char);
        long queued = Interlocked.Add(ref _hostedQueuedBytes, bytes);
        if (queued <= HostedQueueByteCapacity && _channel.Writer.TryWrite(new QueuedWebMessage(message, document, true))) return;

        Interlocked.Add(ref _hostedQueuedBytes, -bytes);
        var exception = new InvalidOperationException(
            "The Blazor message queue of a hosted window is full; the window cannot continue without dropping messages.");
        _logger.LogError(exception,
            "Hosted Blazor delivery faulted. QueueCapacity: {QueueCapacity}, ByteCapacity: {ByteCapacity}",
            _messageQueueCapacity, HostedQueueByteCapacity);
        _channel.Writer.TryComplete(exception);
        documentWindow.FaultWindow(exception);
        throw exception;
    }

    private async Task MessagePump() {
        try {
            while (await _channel.Reader.WaitToReadAsync(_messagePumpShutdown.Token)) {
                while (_channel.Reader.TryRead(out QueuedWebMessage message)) {
                    if (IsDisposingOrDisposed || _messagePumpShutdown.IsCancellationRequested) return;

                    if (!message.Hosted) {
                        await LazyWindow.Value.SendWebMessageAsync(message.Payload, _messagePumpShutdown.Token).ConfigureAwait(false);
                        continue;
                    }
                    try {
                        await Volatile.Read(ref _documentWindow)!
                            .SendWebMessageForDocumentAsync(message.Payload, message.Document, _messagePumpShutdown.Token)
                            .ConfigureAwait(false);
                    }
                    catch (InfiniFrameHostDocumentRetiredException) {
                        _logger.LogDebug("Cancelled outbound Blazor work for retired hosted document {Document}", message.Document);
                    }
                    finally {
                        Interlocked.Add(ref _hostedQueuedBytes, -(long)message.Payload.Length * sizeof(char));
                    }
                }
            }
        }
        catch (ObjectDisposedException ex) {
            _logger.LogDebug(ex, "WebView message pump observed disposed dependencies; stopping");
        }
        catch (ChannelClosedException ex) {
            _logger.LogDebug(ex, "WebView message channel closed; stopping message pump");
        }
        catch (OperationCanceledException) {
            _logger.LogDebug("WebView message pump cancellation requested");
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            _logger.LogError(ex, "Unhandled exception in WebView message pump");
        }
    }

    private readonly record struct QueuedWebMessage(string Payload, ulong Document, bool Hosted);

    protected override async ValueTask DisposeAsyncCore() {
        if (Interlocked.Exchange(ref _disposeStarted, 1) != 0) return;

        _channel.Writer.TryComplete();
        await _messagePumpShutdown.CancelAsync();

        try {
            // Some tests build and dispose of the app without ever creating a native window.
            // In that case, disposing the base WebView manager can flow through dispatch paths
            // that resolve IInfiniFrameWindow and initialize native resources during teardown.
            // Avoid creating a window while disposing of an app that never ran.
            if (LazyWindow.IsValueCreated) {
                await base.DisposeAsyncCore().ConfigureAwait(false);
            }
        }
        finally {
            try {
                await _messagePumpTask.ConfigureAwait(false);
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                _logger.LogWarning(ex, "Message pump faulted during WebView manager shutdown");
            }
            finally {
                _messagePumpShutdown.Dispose();
                Volatile.Write(ref _disposed, 1);
                _logger.LogDebug("WebView manager disposal completed after the message pump stopped");
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
