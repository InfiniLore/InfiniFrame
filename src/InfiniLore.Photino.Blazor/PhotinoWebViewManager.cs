// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using InfiniLore.Photino.Blazor.Utils;
using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace InfiniLore.Photino.Blazor;

public class PhotinoWebViewManager : WebViewManager
{

    // On Windows, we can't use a custom scheme to host the initial HTML,
    // because webview2 won't let you do top-level navigation to such a URL.
    // On Linux/Mac, we must use a custom scheme, because their webviews
    // don't have a way to intercept http:// scheme requests.
    public static readonly string BlazorAppScheme = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "http"
        : "app";

    public static readonly string AppBaseUri = $"{BlazorAppScheme}://localhost/";
    private readonly Channel<string> _channel;
    private readonly PhotinoWindow _window;

    public PhotinoWebViewManager(PhotinoWindow window, IServiceProvider provider, Dispatcher dispatcher,
        IFileProvider fileProvider, JSComponentConfigurationStore jsComponents, IOptions<PhotinoBlazorAppConfiguration> config)
        : base(provider, dispatcher, config.Value.AppBaseUri, fileProvider, jsComponents, config.Value.HostPage)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));

        // Create a scheduler that uses one threads.
        var sts = new SynchronousTaskScheduler();

        _window.WebMessageReceived += (_, message) =>
        {
            // On some platforms, we need to move off the browser UI thread
            Task.Factory.StartNew(m =>
            {
                // TODO: Fix this. Photino should ideally tell us the URL that the message comes from so we
                // know whether to trust it. Currently it's hardcoded to trust messages from any source, including
                // if the webview is somehow navigated to an external URL.
                var messageOriginUrl = new Uri(AppBaseUri);
                
                MessageReceived(messageOriginUrl, (string)m!);
            }, message, CancellationToken.None, TaskCreationOptions.DenyChildAttach, sts);
        };

        //Create channel and start reader
        _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false, AllowSynchronousContinuations = false });
        Task.Run(MessagePump);
    }

    public Stream? HandleWebRequest(object? sender, string? schema, string url, out string? contentType)
    {
        // It would be better if we were told whether or not this is a navigation request, but
        // since we're not, guess.
        var localPath = new Uri(url).LocalPath;
        var hasFileExtension = localPath.LastIndexOf('.') > localPath.LastIndexOf('/');

        //Remove parameters before attempting to retrieve the file. For example: http://localhost/_content/Blazorise/button.js?v=1.0.7.0
        if (url.Contains('?')) url = url.Substring(0, url.IndexOf('?'));

        if (url.StartsWith(AppBaseUri, StringComparison.Ordinal)
            && TryGetResponseContent(url, !hasFileExtension, out _, out _,
                                     out var content, out var headers))
        {
            headers.TryGetValue("Content-Type", out contentType);
            return content;
        }
        contentType = null;
        return null;
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        _window.Load(absoluteUri);
    }

    protected override void SendMessage(string message)
    {
        while (!_channel.Writer.TryWrite(message))
            Thread.Sleep(200);
    }

    private async Task MessagePump()
    {
        var reader = _channel.Reader;
        try
        {
            while (true)
            {
                var message = await reader.ReadAsync();
                await _window.SendWebMessageAsync(message);
            }
        }
        catch (ChannelClosedException) {}
    }

    protected override ValueTask DisposeAsyncCore()
    {
        //complete channel
        try { _channel.Writer.Complete(); }
        catch
        {
            // ignored
        }

        //continue disposing
        return base.DisposeAsyncCore();
    }
}
