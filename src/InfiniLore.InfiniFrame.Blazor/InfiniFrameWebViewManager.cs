// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using InfiniLore.InfiniFrame.Blazor.Utils;

namespace InfiniLore.InfiniFrame.Blazor;
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
    private readonly SynchronousTaskScheduler _syncScheduler = new();

    public InfiniFrameWebViewManager(
        IInfiniFrameWindowBuilder builder,
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        IOptions<InfiniFrameBlazorAppConfiguration> config
    )
        : base(provider, dispatcher, config.Value.AppBaseUri, fileProvider, jsComponents, config.Value.HostPage) {

        builder.RegisterWebMessageReceivedHandler((_, message) => {
            // On some platforms, we need to move off the browser UI thread
            Task.Factory.StartNew(action: m => {
                // TODO: Fix this. Photino should ideally tell us the URL that the message comes from so we
                // know whether to trust it. Currently it's hardcoded to trust messages from any source, including
                // if the webview is somehow navigated to an external URL.
                var messageOriginUrl = new Uri(AppBaseUri);

                MessageReceived(messageOriginUrl, (string)m!);
            }, message, CancellationToken.None, TaskCreationOptions.DenyChildAttach, _syncScheduler);
        });

        LazyWindow = new Lazy<IInfiniFrameWindow>(provider.GetRequiredService<IInfiniFrameWindow>);

        //start reader
        Task.Run(MessagePump);
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
        catch (ChannelClosedException) {}
    }

    protected override ValueTask DisposeAsyncCore() {
        //complete channel
        try { _channel.Writer.Complete(); }
        catch {
            // ignored
        }

        //continue disposing
        return base.DisposeAsyncCore();
    }
}
