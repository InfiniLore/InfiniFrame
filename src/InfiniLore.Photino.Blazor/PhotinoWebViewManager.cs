using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.Photino.Blazor;
public class PhotinoWebViewManager : WebViewManager, IPhotinoWebViewManager {
    private readonly PhotinoWindow _window;
    private readonly PhotinoBlazorAppConfiguration _config;

    public PhotinoWebViewManager(
        PhotinoWindow window,
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        string hostPageRelativePath
    )
        : base(
        provider,
        dispatcher,
        GetAppBaseUri(provider),
        fileProvider,
        jsComponents,
        hostPageRelativePath) {
        _window = window;
        _config = provider.GetService<PhotinoBlazorAppConfiguration>()
            ?? new PhotinoBlazorAppConfiguration();
    }


    private static Uri GetAppBaseUri(IServiceProvider provider) {
        var config = provider.GetService<PhotinoBlazorAppConfiguration>();
        return new Uri(config?.AppBaseUri ?? PhotinoBlazorAppConfiguration.DefaultAppBaseUri);
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

        if (url.StartsWith(AppBaseUri.ToString(), StringComparison.Ordinal)
            && TryGetResponseContent(url, !hasFileExtension, out _, out _,
                                     out Stream content, out IDictionary<string, string> headers)) {
            headers.TryGetValue("Content-Type", out contentType);
            return content;
        }
        
        contentType = null;
        return null;
    }

    /// <summary>
    ///     The configured app base URI
    /// </summary>
    public Uri AppBaseUri => new Uri(_config.AppBaseUri);

    protected override void NavigateCore(Uri absoluteUri) {
        _window.Load(absoluteUri.ToString());
    }

    protected override void SendMessage(string message) {
        _window.SendWebMessage(message);
    }
}
