// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using InfiniFrame.BlazorWebView.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebViewManager : WebViewManager, IInfiniFrameWebViewManager {

    // On Windows, we can't use a custom scheme to host the initial HTML,
    // because webview2 won't let you do top-level navigation to such a URL.
    // On Linux/Mac, we must use a custom scheme, because their webviews
    // don't have a way to intercept http:// scheme requests.
    public static readonly string BlazorAppScheme = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "http"
        : "app";

    public static readonly string AppBaseUri = $"{BlazorAppScheme}://localhost/";
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false, AllowSynchronousContinuations = false });
    private Lazy<IInfiniFrameWindow> LazyWindow { get; }
    private Lazy<ILogger<InfiniFrameWebViewManager>?> LazyLogger { get; }
    private readonly SynchronousTaskScheduler _syncScheduler = new();
    private readonly Task _messagePumpTask;
    private readonly Uri _trustedOrigin;

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
        _trustedOrigin = config.Value.AppBaseUri;

        builder.RegisterWebMessageReceivedHandler((_, message) => {
            string? origin = InfiniFrameWebMessageContext.CurrentOrigin;

            // On some platforms, we need to move off the browser UI thread
            Task.Factory.StartNew(
                action: state => HandleWebMessage(((string Message, string? Origin))state!),
                state: (Message: message, Origin: origin),
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.DenyChildAttach,
                scheduler: _syncScheduler);
        });

        LazyWindow = new Lazy<IInfiniFrameWindow>(provider.GetRequiredService<IInfiniFrameWindow>);
        LazyLogger = new Lazy<ILogger<InfiniFrameWebViewManager>?>(() => provider.GetService<ILogger<InfiniFrameWebViewManager>>());

        // Start the reader and observe/await it during disposal.
        _messagePumpTask = Task.Run(MessagePump);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private void HandleWebMessage((string Message, string? Origin) state) {
        if (string.IsNullOrWhiteSpace(state.Origin)) {
            LazyLogger.Value?.LogWarning("Rejected web message because origin is missing or unknown.");
            return;
        }

        if (!Uri.TryCreate(state.Origin, UriKind.Absolute, out Uri? messageOriginUrl)) {
            LazyLogger.Value?.LogWarning("Rejected web message because origin parsing failed. Origin: {Origin}", state.Origin);
            return;
        }

        if (!IsTrustedOrigin(messageOriginUrl, _trustedOrigin)) {
            LazyLogger.Value?.LogWarning(
                "Rejected web message due to origin mismatch. Origin: {MessageOrigin}, TrustedOrigin: {TrustedOrigin}",
                messageOriginUrl,
                _trustedOrigin);
            return;
        }

        MessageReceived(messageOriginUrl, state.Message);
    }

    private static bool IsTrustedOrigin(Uri messageOrigin, Uri trustedOrigin) {
        return string.Equals(messageOrigin.Scheme, trustedOrigin.Scheme, StringComparison.OrdinalIgnoreCase)
               && string.Equals(messageOrigin.Host, trustedOrigin.Host, StringComparison.OrdinalIgnoreCase)
               && messageOrigin.Port == trustedOrigin.Port;
    }

    public Stream? HandleWebRequest(object? sender, string? schema, string? url, out string? contentType) {
        if (url is null) {
            contentType = null;
            return null;// TODO: Handle this better.
        }

        // It would be better if we were told whether this is a navigation request, but
        // since we're not, guess.
        string localPath = new Uri(url).LocalPath;
        bool hasFileExtension = localPath.LastIndexOf('.') > localPath.LastIndexOf('/');

        //Remove parameters before attempting to retrieve the file. For example: http://localhost/_content/Blazorise/button.js?v=1.0.7.0
        if (url.Contains('?')) url = url[..url.IndexOf('?')];

        if (url.StartsWith(AppBaseUri, StringComparison.Ordinal)
            && TryGetResponseContent(url, !hasFileExtension, out _, out _,
                out Stream content, out IDictionary<string, string> headers)) {
            headers.TryGetValue("Content-Type", out contentType);
            return content;
        }

        contentType = null;
        return null;
    }

    protected override void NavigateCore(Uri absoluteUri) {
        LazyWindow.Value.Load(absoluteUri);// TODO handle exceptions
    }

    protected override void SendMessage(string message) {
        while (!_channel.Writer.TryWrite(message)) {
            Thread.Sleep(200);
        }
    }

    private async Task MessagePump() {
        ChannelReader<string> reader = _channel.Reader;
        try {
            while (true) {
                string message = await reader.ReadAsync();
                await LazyWindow.Value.SendWebMessageAsync(message);
            }
        }
        catch (ChannelClosedException) {
            // ignored
        }
        catch (OperationCanceledException) {
            // ignored
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            LazyLogger.Value?.LogError(ex, "Unhandled exception in WebView message pump.");
            throw;
        }
    }

    protected override async ValueTask DisposeAsyncCore() {
        //complete channel
        try { _channel.Writer.Complete(); }
        catch (ChannelClosedException) {
            // ignored
        }

        try {
            await _messagePumpTask;
        }
        catch (ChannelClosedException) {
            // ignored
        }

        //continue disposing
        await base.DisposeAsyncCore();
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
