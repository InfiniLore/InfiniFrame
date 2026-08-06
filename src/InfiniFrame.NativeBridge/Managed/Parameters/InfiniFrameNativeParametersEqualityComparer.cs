// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Compares two <see cref="InfiniFrameNativeParameters" /> instances for value equality,
///     ignoring callback handler fields.
/// </summary>
internal sealed class InfiniFrameNativeParametersEqualityComparer : IEqualityComparer<InfiniFrameNativeParameters> {
    /// <summary>
    ///     Singleton instance of the equality comparer.
    /// </summary>
    internal static readonly InfiniFrameNativeParametersEqualityComparer Instance = new();

    private InfiniFrameNativeParametersEqualityComparer() { }

    /// <summary>
    ///     Determines whether two <see cref="InfiniFrameNativeParameters" /> instances are equal
    ///     by comparing all value fields.
    /// </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The second instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(InfiniFrameNativeParameters x, InfiniFrameNativeParameters y) {
        // Handlers are not checked because they are set by the constructor and are not user-configurable.
        // x.ClosingHandler == y.ClosingHandler
        // && x.ClosedHandler == y.ClosedHandler
        // && x.FocusInHandler == y.FocusInHandler
        // && x.FocusOutHandler == y.FocusOutHandler
        // && x.ResizedHandler == y.ResizedHandler
        // && x.MaximizedHandler == y.MaximizedHandler
        // && x.RestoredHandler == y.RestoredHandler
        // && x.MinimizedHandler == y.MinimizedHandler
        // && x.MovedHandler == y.MovedHandler
        // && x.WebMessageReceivedHandler == y.WebMessageReceivedHandler
        // && x.DebugEventHandler == y.DebugEventHandler
        // && x.CustomSchemeHandler == y.CustomSchemeHandler

        if (!x.CustomSchemeNames.AsSpan().SequenceEqual(y.CustomSchemeNames.AsSpan())) return false;
        if (x.StartString != y.StartString) return false;
        if (x.StartUrl != y.StartUrl) return false;
        if (x.Title != y.Title) return false;
        if (x.WindowIconFile != y.WindowIconFile) return false;
        if (x.TemporaryFilesPath != y.TemporaryFilesPath) return false;
        if (x.UserAgent != y.UserAgent) return false;
        if (x.BrowserControlInitParameters != y.BrowserControlInitParameters) return false;
        if (x.WebView2RuntimePath != y.WebView2RuntimePath) return false;
        if (x.NotificationRegistrationId != y.NotificationRegistrationId) return false;
        if (x.WindowsAppUserModelId != y.WindowsAppUserModelId) return false;
        if (x.RemoteDebuggingPort != y.RemoteDebuggingPort) return false;
        if (x.NativeParent != y.NativeParent) return false;
        if (x.Left != y.Left) return false;
        if (x.Top != y.Top) return false;
        if (x.Width != y.Width) return false;
        if (x.Height != y.Height) return false;
        if (x.Zoom != y.Zoom) return false;
        if (x.MinWidth != y.MinWidth) return false;
        if (x.MinHeight != y.MinHeight) return false;
        if (x.MaxWidth != y.MaxWidth) return false;
        if (x.MaxHeight != y.MaxHeight) return false;
        if (x.CenterOnInitialize != y.CenterOnInitialize) return false;
        if (x.Chromeless != y.Chromeless) return false;
        if (x.Transparent != y.Transparent) return false;
        if (x.ContextMenuEnabled != y.ContextMenuEnabled) return false;
        if (x.DevToolsEnabled != y.DevToolsEnabled) return false;
        if (x.WebInspectorEnabled != y.WebInspectorEnabled) return false;
        if (x.FullScreen != y.FullScreen) return false;
        if (x.Maximized != y.Maximized) return false;
        if (x.Minimized != y.Minimized) return false;
        if (x.Resizable != y.Resizable) return false;
        if (x.Topmost != y.Topmost) return false;
        if (x.UseOsDefaultLocation != y.UseOsDefaultLocation) return false;
        if (x.UseOsDefaultSize != y.UseOsDefaultSize) return false;
        if (x.GrantBrowserPermissions != y.GrantBrowserPermissions) return false;
        if (x.MediaAutoplayEnabled != y.MediaAutoplayEnabled) return false;
        if (x.FileSystemAccessEnabled != y.FileSystemAccessEnabled) return false;
        if (x.WebSecurityEnabled != y.WebSecurityEnabled) return false;
        if (x.JavascriptClipboardAccessEnabled != y.JavascriptClipboardAccessEnabled) return false;
        if (x.MediaStreamEnabled != y.MediaStreamEnabled) return false;
        if (x.SmoothScrollingEnabled != y.SmoothScrollingEnabled) return false;
        if (x.IgnoreCertificateErrorsEnabled != y.IgnoreCertificateErrorsEnabled) return false;
        if (x.StatusBarEnabled != y.StatusBarEnabled) return false;
        if (x.NotificationsEnabled != y.NotificationsEnabled) return false;
        if (x.DefaultNotificationIcon != y.DefaultNotificationIcon) return false;
        if (x.MenuBarJson != y.MenuBarJson) return false;
        if (x.Size != y.Size) return false;
        if (x.ZoomEnabled != y.ZoomEnabled) return false;

        return true;
    }

    /// <summary>
    ///     Returns a hash code for the specified <see cref="InfiniFrameNativeParameters" /> instance
    ///     based on its value fields.
    /// </summary>
    /// <param name="obj">The instance to hash.</param>
    /// <returns>A hash code value.</returns>
    public int GetHashCode(InfiniFrameNativeParameters obj) {
        var hashCode = new HashCode();
        hashCode.Add(obj.StartString);
        hashCode.Add(obj.StartUrl);
        hashCode.Add(obj.Title);
        hashCode.Add(obj.WindowIconFile);
        hashCode.Add(obj.TemporaryFilesPath);
        hashCode.Add(obj.UserAgent);
        hashCode.Add(obj.BrowserControlInitParameters);
        hashCode.Add(obj.WebView2RuntimePath);
        hashCode.Add(obj.NotificationRegistrationId);
        hashCode.Add(obj.WindowsAppUserModelId);
        hashCode.Add(obj.RemoteDebuggingPort);
        hashCode.Add(obj.NativeParent);

        foreach (IntPtr ptr in obj.CustomSchemeNames) {
            hashCode.Add(ptr);
        }

        hashCode.Add(obj.Left);
        hashCode.Add(obj.Top);
        hashCode.Add(obj.Width);
        hashCode.Add(obj.Height);
        hashCode.Add(obj.Zoom);
        hashCode.Add(obj.MinWidth);
        hashCode.Add(obj.MinHeight);
        hashCode.Add(obj.MaxWidth);
        hashCode.Add(obj.MaxHeight);
        hashCode.Add(obj.CenterOnInitialize);
        hashCode.Add(obj.Chromeless);
        hashCode.Add(obj.Transparent);
        hashCode.Add(obj.ContextMenuEnabled);
        hashCode.Add(obj.DevToolsEnabled);
        hashCode.Add(obj.WebInspectorEnabled);
        hashCode.Add(obj.FullScreen);
        hashCode.Add(obj.Maximized);
        hashCode.Add(obj.Minimized);
        hashCode.Add(obj.Resizable);
        hashCode.Add(obj.Topmost);
        hashCode.Add(obj.UseOsDefaultLocation);
        hashCode.Add(obj.UseOsDefaultSize);
        hashCode.Add(obj.GrantBrowserPermissions);
        hashCode.Add(obj.MediaAutoplayEnabled);
        hashCode.Add(obj.FileSystemAccessEnabled);
        hashCode.Add(obj.WebSecurityEnabled);
        hashCode.Add(obj.JavascriptClipboardAccessEnabled);
        hashCode.Add(obj.MediaStreamEnabled);
        hashCode.Add(obj.SmoothScrollingEnabled);
        hashCode.Add(obj.IgnoreCertificateErrorsEnabled);
        hashCode.Add(obj.StatusBarEnabled);
        hashCode.Add(obj.NotificationsEnabled);
        hashCode.Add(obj.DefaultNotificationIcon);
        hashCode.Add(obj.MenuBarJson);
        hashCode.Add(obj.Size);
        hashCode.Add(obj.ZoomEnabled);
        return hashCode.ToHashCode();
    }
}