// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.Utils;
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
    private readonly InfiniFrameUriSecurityPolicy _uriSecurityPolicy;

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

        LazyWindow = new Lazy<IInfiniFrameWindow>(provider.GetRequiredService<IInfiniFrameWindow>);
        LazyLogger = new Lazy<ILogger<InfiniFrameWebViewManager>?>(provider.GetService<ILogger<InfiniFrameWebViewManager>>);

        builder.RegisterWebMessageReceivedHandler((_, message) => {
            string? origin = InfiniFrameWebMessageContext.CurrentOrigin;

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
    public Stream? HandleWebRequest(
        object? sender,
        string? schema,
        string? url,
        out string? contentType
    ) {
        contentType = null;

        if (string.IsNullOrWhiteSpace(url)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request because URL is null or empty. Schema: {Schema}",
                schema
            );
            return null;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? requestUri)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request because URL parsing failed. Url: {Url}, Schema: {Schema}",
                url, schema
            );
            return null;
        }

        // ---------------------------------------------------------------------
        // IMPORTANT FIX: allow Blazor + app internal scheme BEFORE validation
        // ---------------------------------------------------------------------
        if (requestUri.Scheme == BlazorAppScheme) {
            // no security policy, no warnings — this is framework/app internal traffic

            string localPath = requestUri.LocalPath;
            bool hasFileExtension = Path.HasExtension(localPath);

            Uri sanitizedUri = new UriBuilder(requestUri) {
                Query = string.Empty,
                Fragment = string.Empty
            }.Uri;

            if (!TryGetResponseContent(sanitizedUri.AbsoluteUri, !hasFileExtension, out _, out _, out Stream content, out IDictionary<string, string> headers))
                return null;

            headers.TryGetValue("Content-Type", out contentType);
            contentType ??= GetFallbackContentType(sanitizedUri.LocalPath);
            return content;

        }

        // ---------------------------------------------------------------------
        // External / non-Blazor traffic (secure path)
        // ---------------------------------------------------------------------
        if (!_uriSecurityPolicy.IsNavigationSchemeAllowed(requestUri.Scheme)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request due to disallowed URI scheme. Scheme: {Scheme}, Url: {Url}",
                requestUri.Scheme,
                requestUri);
            return null;
        }

        if (!_uriSecurityPolicy.IsTrustedOrigin(requestUri)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request due to untrusted origin. RequestOrigin: {RequestOrigin}, TrustedOrigins: {TrustedOrigins}",
                requestUri,
                _uriSecurityPolicy.TrustedOrigins);
            return null;
        }

        if (!string.IsNullOrWhiteSpace(schema) &&
            !string.Equals(schema, requestUri.Scheme, StringComparison.OrdinalIgnoreCase)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web request due to schema mismatch. ReportedSchema: {ReportedSchema}, UriScheme: {UriScheme}, Url: {Url}",
                schema,
                requestUri.Scheme,
                url);
            return null;
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
            headers2.TryGetValue("Content-Type", out contentType);
            contentType ??= GetFallbackContentType(sanitizedUri2.LocalPath);
            return content2;
        }

        LazyLogger.Value?.LogWarning(
            "No web content found for trusted URL. Url: {Url}",
            sanitizedUri2);

        return null;
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
        LazyWindow.Value.Load(absoluteUri);
    }

    protected override void SendMessage(string message) {
        while (!_channel.Writer.TryWrite(message)) {
            Thread.Sleep(200);
        }
    }

    private async Task MessagePump() {
        try {
            while (await _channel.Reader.ReadAsync() is { } message) {
                await LazyWindow.Value.SendWebMessageAsync(message);
            }
        }
        catch (ChannelClosedException) {}
        catch (OperationCanceledException) {}
        catch (Exception ex) when (IsNonFatalException(ex)) {
            LazyLogger.Value?.LogError(ex, "Unhandled exception in WebView message pump.");
            throw;
        }
    }

    protected override async ValueTask DisposeAsyncCore() {
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

        await base.DisposeAsyncCore();
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);

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
