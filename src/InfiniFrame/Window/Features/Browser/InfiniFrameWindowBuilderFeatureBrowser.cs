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
public class InfiniFrameWindowBuilderFeatureBrowser : IInfiniFrameWindowBuilderFeatureBrowser {
    private readonly Guid _defaultTemporaryFilesPathId = Guid.NewGuid();
    private bool _temporaryFilesPathExplicitlyAssigned;

    internal bool TemporaryFilesPathExplicitlyAssigned => _temporaryFilesPathExplicitlyAssigned;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsContextMenuEnabled"/>
    public bool IsContextMenuEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsMediaAutoplayEnabled"/>
    public bool IsMediaAutoplayEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.UserAgent"/>
    public string? UserAgent { get; private set; } = "InfiniFrame WebView";

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsFileSystemAccessEnabled"/>
    public bool IsFileSystemAccessEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsWebSecurityEnabled"/>
    public bool IsWebSecurityEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsJavascriptClipboardAccessEnabled"/>
    public bool IsJavascriptClipboardAccessEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsMediaStreamEnabled"/>
    public bool IsMediaStreamEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsIgnoreCertificateErrorsEnabled"/>
    public bool IsIgnoreCertificateErrorsEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.GrantBrowserPermissions"/>
    public bool GrantBrowserPermissions { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.IsSmoothScrollingEnabled"/>
    public bool IsSmoothScrollingEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.BrowserControlInitParameters"/>
    public string? BrowserControlInitParameters { get; private set; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.TemporaryFilesPath"/>
    public string TemporaryFilesPath { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameWindowBuilderFeatureBrowser() {
        TemporaryFilesPath = CreateDefaultTemporaryFilesPath(_defaultTemporaryFilesPathId);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableContextMenu"/>
    public void EnableContextMenu(bool enabled) {
        IsContextMenuEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableMediaAutoplay"/>
    public void EnableMediaAutoplay(bool enabled) {
        IsMediaAutoplayEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.SetUserAgent"/>
    public void SetUserAgent(string? userAgent) {
        if (string.IsNullOrWhiteSpace(userAgent)) userAgent = string.Empty;
        UserAgent = userAgent;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableFileSystemAccess"/>
    public void EnableFileSystemAccess(bool enabled) {
        IsFileSystemAccessEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableWebSecurity"/>
    public void EnableWebSecurity(bool enabled) {
        IsWebSecurityEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableJavascriptClipboardAccess"/>
    public void EnableJavascriptClipboardAccess(bool enabled) {
        IsJavascriptClipboardAccessEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableMediaStream"/>
    public void EnableMediaStream(bool enabled) {
        IsMediaStreamEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableIgnoreCertificateErrors"/>
    public void EnableIgnoreCertificateErrors(bool enabled) {
        IsIgnoreCertificateErrorsEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableBrowserPermissions"/>
    public void EnableBrowserPermissions(bool enabled) {
        GrantBrowserPermissions = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.EnableSmoothScrolling"/>
    public void EnableSmoothScrolling(bool enabled) {
        IsSmoothScrollingEnabled = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.SetBrowserControlInitParameters"/>
    public void SetBrowserControlInitParameters(string? parameters) {
        BrowserControlInitParameters = parameters;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureBrowser.SetTemporaryFilesPath"/>
    public void SetTemporaryFilesPath(string path) {
        TemporaryFilesPath = path;
        _temporaryFilesPathExplicitlyAssigned = true;
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
        parameters.BrowserControlInitParameters = BrowserControlInitParameters;
        parameters.TemporaryFilesPath = ResolveTemporaryFilesPath();
    }

    internal string ResolveTemporaryFilesPath(Guid windowId) {
        if (_temporaryFilesPathExplicitlyAssigned) return TemporaryFilesPath;

        return CreateDefaultTemporaryFilesPath(windowId);
    }

    private string ResolveTemporaryFilesPath() {
        if (_temporaryFilesPathExplicitlyAssigned) return TemporaryFilesPath;

        return CreateDefaultTemporaryFilesPath(_defaultTemporaryFilesPathId);
    }

    private static string CreateDefaultTemporaryFilesPath(Guid id)
        => Path.Join(
            Path.GetTempPath(),
            "infiniframe",
            Environment.ProcessId.ToString(),
            id.ToString("N")
        );
}
