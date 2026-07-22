// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class BrowserWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureBrowser> {
    public override string FeatureName => "browser";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IInfiniFrameWindowFeatureBrowser SelectFeature(IInfiniFrameWindowFeatures features) 
        => features.Browser;

    protected override object? Get(IInfiniFrameWindowFeatureBrowser feature, string command, JsonElement? args) => command switch {
        "isContextMenuEnabled" => feature.IsContextMenuEnabled,
        "isMediaAutoplayEnabled" => feature.IsMediaAutoplayEnabled,
        "userAgent" => feature.UserAgent,
        "isFileSystemAccessEnabled" => feature.IsFileSystemAccessEnabled,
        "isWebSecurityEnabled" => feature.IsWebSecurityEnabled,
        "isJavascriptClipboardAccessEnabled" => feature.IsJavascriptClipboardAccessEnabled,
        "isMediaStreamEnabled" => feature.IsMediaStreamEnabled,
        "isIgnoreCertificateErrorsEnabled" => feature.IsIgnoreCertificateErrorsEnabled,
        "grantBrowserPermissions" => feature.GrantBrowserPermissions,
        "isSmoothScrollingEnabled" => feature.IsSmoothScrollingEnabled,
        "browserControlInitParameters" => feature.BrowserControlInitParameters,
        _ => throw Unsupported(command)
    };

    protected override void Post(IInfiniFrameWindowFeatureBrowser feature, string command, JsonElement? args) {
        switch (command) {
            case "enableContextMenu": feature.EnableContextMenu(Arg(args, "enabled", true)); return;
            case "enableMediaAutoplay": feature.EnableMediaAutoplay(Arg(args, "enabled", true)); return;
            case "setUserAgent": feature.SetUserAgent(Arg<string?>(args, "userAgent", null)); return;
            case "win32SetWebView2Path": feature.Win32SetWebView2Path(Required<string>(args, "path")); return;
            case "clearBrowserAutoFill": feature.ClearBrowserAutoFill(); return;
            default: throw Unsupported(command);
        }
    }
}
