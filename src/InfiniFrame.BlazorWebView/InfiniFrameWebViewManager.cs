// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.Utilities;
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

    private readonly Channel<string> _channel =
        Channel.CreateUnbounded<string>(new UnboundedChannelOptions {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

    private readonly Task _messagePumpTask;
    private readonly SynchronousTaskScheduler _syncScheduler = new();
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
        _uriSecurityPolicy = InfiniFrameUriSecurityPolicyRegistry
            .GetForBuilder(builder)
            .WithTrustedOrigin(config.Value.AppBaseUri);

        // ReSharper disable once ConvertClosureToMethodGroup
        LazyWindow = new Lazy<IInfiniFrameWindow>(() => provider.GetRequiredService<IInfiniFrameWindow>());
        // ReSharper disable once ConvertClosureToMethodGroup
        LazyLogger = new Lazy<ILogger<InfiniFrameWebViewManager>?>(() => provider.GetService<ILogger<InfiniFrameWebViewManager>>());

        builder.RegisterWebMessageReceivedHandler((_, message, origin) => {
            LazyLogger.Value?.LogDebug(
                "Web message callback from native. Origin: {Origin}, Message: {Message}",
                origin,
                message);

            Task.Factory.StartNew(
                state => HandleWebMessage(((string Message, string? Origin))state!),
                (Message: message, Origin: origin),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                _syncScheduler);
        });

        _messagePumpTask = Task.Run(MessagePump);
    }

    private Lazy<IInfiniFrameWindow> LazyWindow { get; }
    private Lazy<ILogger<InfiniFrameWebViewManager>?> LazyLogger { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Web Requests
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWebViewManager.HandleWebRequest"/>
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
            out IDictionary<string, string> headers2)) {
            
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

        MessageReceived(messageOriginUrl, state.Message);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Navigation
    // -----------------------------------------------------------------------------------------------------------------
    protected override void NavigateCore(Uri absoluteUri) {
        LazyWindow.Value.Features.PageNavigation.Load(absoluteUri);
    }

    protected override void SendMessage(string message) {
        if (_channel.Writer.TryWrite(message)) return;
        LazyLogger.Value?.LogDebug("Skipping WebView message because the message channel is closed.");
    }

    private async Task MessagePump() {
        try {
            while (await _channel.Reader.ReadAsync() is { } message) {
                await LazyWindow.Value.SendWebMessageAsync(message);
            }
        }
        catch (ChannelClosedException ex) {
            LazyLogger.Value?.LogDebug(ex, "WebView message channel closed; stopping message pump.");
        }
        catch (OperationCanceledException) {
            LazyLogger.Value?.LogDebug("WebView message pump cancellation requested.");
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            LazyLogger.Value?.LogError(ex, "Unhandled exception in WebView message pump.");
            throw;
        }
    }

    protected override async ValueTask DisposeAsyncCore() {
        try {
            await base.DisposeAsyncCore();
        }
        finally {
            try { _channel.Writer.Complete(); }
            catch (ChannelClosedException ex) {
                LazyLogger.Value?.LogDebug(ex, "Channel was already closed during dispose.");
            }

            try {
                await _messagePumpTask.WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException) {
                LazyLogger.Value?.LogWarning(
                    "Timed out while waiting for WebView message pump shutdown.");
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
