// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configures browser-control-related settings for an <see cref="IInfiniFrameWindow" />, including context menu,
///     media autoplay, user agent, security, permissions, and other WebView options.
/// </summary>
public class BrowserInfiniFrameWindowBuilderFeature : IBrowserInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsContextMenuEnabled" />
    public bool IsContextMenuEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsMediaAutoplayEnabled" />
    public bool IsMediaAutoplayEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.UserAgent" />
    public string? UserAgent { get; private set; } = "InfiniFrame WebView";

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsFileSystemAccessEnabled" />
    public bool IsFileSystemAccessEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsWebSecurityEnabled" />
    public bool IsWebSecurityEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsJavascriptClipboardAccessEnabled" />
    public bool IsJavascriptClipboardAccessEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsMediaStreamEnabled" />
    public bool IsMediaStreamEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsIgnoreCertificateErrorsEnabled" />
    public bool IsIgnoreCertificateErrorsEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.GrantBrowserPermissions" />
    public bool GrantBrowserPermissions { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsSmoothScrollingEnabled" />
    public bool IsSmoothScrollingEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsStatusBarEnabled" />
    public bool IsStatusBarEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.IsBrowserShortcutsEnabled" />
    public bool IsBrowserShortcutsEnabled { get; private set; } = true;

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.BrowserControlInitParameters" />
    public string? BrowserControlInitParameters { get; private set; }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.TemporaryFilesPath" />
    public string TemporaryFilesPath { get; private set; } = Path.Join(
        Path.GetTempPath(),
        "infiniframe",
        Environment.ProcessId.ToString()
    );

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.WebView2RuntimePath" />
    public string? WebView2RuntimePath { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableContextMenu" />
    public void EnableContextMenu(bool enabled) {
        IsContextMenuEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableMediaAutoplay" />
    public void EnableMediaAutoplay(bool enabled) {
        IsMediaAutoplayEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.SetUserAgent" />
    public void SetUserAgent(string? userAgent) {
        if (string.IsNullOrWhiteSpace(userAgent)) {
            UserAgent = string.Empty;
            return;
        }

        // Strip control characters and limit length to prevent header injection.
        string sanitized = new string(userAgent.Where(static c => !char.IsControl(c)).ToArray());
        UserAgent = sanitized.Length > 1024 ? sanitized[..1024] : sanitized;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableFileSystemAccess" />
    public void EnableFileSystemAccess(bool enabled) {
        IsFileSystemAccessEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableWebSecurity" />
    public void EnableWebSecurity(bool enabled) {
        IsWebSecurityEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableJavascriptClipboardAccess" />
    public void EnableJavascriptClipboardAccess(bool enabled) {
        IsJavascriptClipboardAccessEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableMediaStream" />
    public void EnableMediaStream(bool enabled) {
        IsMediaStreamEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableIgnoreCertificateErrors" />
    public void EnableIgnoreCertificateErrors(bool enabled) {
        IsIgnoreCertificateErrorsEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableBrowserPermissions" />
    public void EnableBrowserPermissions(bool enabled) {
        GrantBrowserPermissions = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableSmoothScrolling" />
    public void EnableSmoothScrolling(bool enabled) {
        IsSmoothScrollingEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableStatusBar" />
    public void EnableStatusBar(bool enabled) {
        IsStatusBarEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.EnableBrowserShortcuts" />
    public void EnableBrowserShortcuts(bool enabled) {
        IsBrowserShortcutsEnabled = enabled;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.SetBrowserControlInitParameters" />
    public void SetBrowserControlInitParameters(string? parameters) {
        if (string.IsNullOrWhiteSpace(parameters)) {
            BrowserControlInitParameters = null;
            return;
        }

        if (parameters.Any(static c => char.IsControl(c)))
            throw new ArgumentException("Browser control init parameters must not contain control characters.", nameof(parameters));
        if (parameters.Length > 4096)
            throw new ArgumentException("Browser control init parameters must not exceed 4096 characters.", nameof(parameters));

        BrowserControlInitParameters = parameters;
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.SetTemporaryFilesPath" />
    public void SetTemporaryFilesPath(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        TemporaryFilesPath = Path.GetFullPath(path);
    }

    /// <inheritdoc cref="IBrowserInfiniFrameWindowBuilderFeature.SetWebView2RuntimePath" />
    public void SetWebView2RuntimePath(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        WebView2RuntimePath = Path.GetFullPath(path);
    }

    /// <summary>
    ///     Applies all browser feature settings to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to populate.</param>
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.ContextMenuEnabled = IsContextMenuEnabled;
        parameters.MediaAutoplayEnabled = IsMediaAutoplayEnabled;
        parameters.UserAgent = UserAgent;
        parameters.FileSystemAccessEnabled = IsFileSystemAccessEnabled;
        parameters.WebSecurityEnabled = IsWebSecurityEnabled;
        parameters.JavascriptClipboardAccessEnabled = IsJavascriptClipboardAccessEnabled;
        parameters.MediaStreamEnabled = IsMediaStreamEnabled;
        parameters.IgnoreCertificateErrorsEnabled = IsIgnoreCertificateErrorsEnabled;
        parameters.GrantBrowserPermissions = GrantBrowserPermissions;
        parameters.SmoothScrollingEnabled = IsSmoothScrollingEnabled;
        parameters.StatusBarEnabled = IsStatusBarEnabled;
        parameters.BrowserShortcutsEnabled = IsBrowserShortcutsEnabled;
        parameters.BrowserControlInitParameters = BrowserControlInitParameters;
        parameters.TemporaryFilesPath = TemporaryFilesPath;
    }
}
