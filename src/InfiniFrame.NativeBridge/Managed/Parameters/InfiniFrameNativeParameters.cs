// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge.Delegates;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// These are the parameter names that are passed to InfiniFrame.Native.
// Field order defines the ABI layout shared with the native (C++) side via LayoutKind.Sequential.
// DO NOT reorder fields, append new fields before Size and update Size accordingly.
/// <summary>
///     Represents the parameters used to configure and initialize a native InfiniFrame window.
///     Passed to the native layer as a sequentially laid-out struct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct InfiniFrameNativeParameters() {
    // ── Content strings ────────────────────────────────────────────────────

    /// <summary>
    ///     EITHER StartString or StartUrl Must be specified: Browser control will render this HTML string when
    ///     initialized. Default is none.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? StartString;

    /// <summary>
    ///     EITHER StartString or StartUrl Must be specified: Browser control will navigate to this URL when initialized.
    ///     Default is none.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? StartUrl;

    // ── Window identity / appearance strings ───────────────────────────────

    ///<summary>OPTIONAL: Appears on the title bar of the native window. Default is none.</summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? Title;

    /// <summary>
    ///     WINDOWS AND LINUX ONLY: OPTIONAL: Path to a local file or a URL. Icon appears on the title bar of the native
    ///     window (if supported). Default is none.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? WindowIconFile;

    ///<summary>WINDOWS: OPTIONAL: Path to store temp files for browser control. Defaults is the user's AppDataLocal folder.</summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? TemporaryFilesPath;

    ///<summary>OPTIONAL: Changes the user agent on the browser control at initialization.</summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? UserAgent;

    /// <summary>
    ///     OPTIONAL:
    ///     WINDOWS: WebView2 specific string.
    ///     https://peter.sh/experiments/chromium-command-line-switches/
    ///     https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49
    ///     https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
    ///     LINUX: Webkit2Gtk specific string.
    ///     https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html
    ///     https://lazka.github.io/pgi-docs/WebKit2-4.0/classes/Settings.html
    ///     MAC: Webkit specific string.
    ///     https://developer.apple.com/documentation/webkit/wkwebviewconfiguration?language=objc
    ///     https://developer.apple.com/documentation/webkit/wkpreferences?language=objc
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? BrowserControlInitParameters;

    /// <summary>
    ///     WINDOWS ONLY: OPTIONAL: Path to an extracted fixed-version WebView2 runtime used when the window is created.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? WebView2RuntimePath;

    ///<summary>WINDOWS: OPTIONAL: Registers the application for toast notifications. If not provided, use Window Title.</summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? NotificationRegistrationId;

    ///<summary>WINDOWS: OPTIONAL: Explicit application identity used by the taskbar for grouping and pinning.</summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? WindowsAppUserModelId;

    /// <summary>
    ///     OPTIONAL: Default icon path applied to notifications when IconPath is not specified.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? DefaultNotificationIcon;

    // ── Runtime configuration ──────────────────────────────────────────────

    ///<summary>OPTIONAL: Windows-only remote debugging port for loopback endpoint. 0 disables remote debugging.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int RemoteDebuggingPort;

    // ── Parent window ──────────────────────────────────────────────────────

    /// <summary>
    ///     OPTIONAL: If the native window is created from another native window, this is the pointer to the parent window.
    /// </summary>
    internal IntPtr NativeParent;

    // ── Event callbacks ────────────────────────────────────────────────────

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppClosingDelegate? ClosingHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppClosedDelegate? ClosedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppFocusInDelegate? FocusInHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppFocusOutDelegate? FocusOutHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppResizedDelegate? ResizedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppMaximizedDelegate? MaximizedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppRestoredDelegate? RestoredHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppMinimizedDelegate? MinimizedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppMovedDelegate? MovedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppWebMessageReceivedDelegate? WebMessageReceivedHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppDebugEventDelegate? DebugEventHandler;

    // ── Custom scheme support ──────────────────────────────────────────────

    ///<summary>OPTIONAL: Names of custom URL Schemes. E.g. 'app', 'custom'. Array length must be 16. Default is none.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    internal IntPtr[] CustomSchemeNames = new IntPtr[16];

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppWebResourceRequestedDelegate? CustomSchemeHandler;

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppNavigationStartingDelegate? NavigationStartingHandler;

    // ── Drag-and-drop ──────────────────────────────────────────────────────

    ///<summary>Set by InfiniFrameOptionsBuilder</summary>
    [MarshalAs(UnmanagedType.FunctionPtr)]
    internal CppFileDroppedDelegate? FileDroppedHandler;

    ///<summary>OPTIONAL: Enables native drag-and-drop file handling. Default is false.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool DragDropEnabled;

    // ── Window geometry ────────────────────────────────────────────────────

    ///<summary>OPTIONAL: Initial window position in pixels. Default is 0. Can be overridden with UseOsDefaultLocation.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int Left;

    ///<summary>OPTIONAL: Initial window position in pixels. Default is 0. Can be overridden with UseOsDefaultLocation.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int Top;

    ///<summary>OPTIONAL: Initial window size in pixels. Default is 0. Can be overridden with UseOsDefaultSize.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int Width;

    ///<summary>OPTIONAL: Initial window size in pixels. Default is. Can be overridden with UseOsDefaultSize.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int Height;

    ///<summary>OPTIONAL: Initial zoom level of the native browser control. e.g.100 = 100%  Default is 100.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int Zoom;

    ///<summary>OPTIONAL: Initial minimum window width in pixels.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int MinWidth;

    ///<summary>OPTIONAL: Initial minimum window height in pixels.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int MinHeight;

    ///<summary>OPTIONAL: Initial maximum window width in pixels.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int MaxWidth;

    ///<summary>OPTIONAL: Initial maximum window height in pixels.</summary>
    [MarshalAs(UnmanagedType.I4)]
    internal int MaxHeight;

    // ── Behavior flags ─────────────────────────────────────────────────────

    /// <summary>
    ///     OPTIONAL: If true, the native window appears in centered on screen. Left and Top properties are ignored. Default
    ///     is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool CenterOnInitialize;

    /// <summary>
    ///     OPTIONAL: If true, the window is created without a title bar or borders. This allows owner-drawn title bars and
    ///     borders. Default is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Chromeless;

    /// <summary>
    ///     OPTIONAL: If true, the window can be displayed with a transparent background. Chromeless must be set to true. HTML
    ///     document's body background must have alpha-based value. Default is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Transparent;

    ///<summary>OPTIONAL: If true, the user can access the browser control's context menu. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool ContextMenuEnabled;

    /// <summary>
    ///     OPTIONAL: If true, the user can zoom the browser control. Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool ZoomEnabled;

    ///<summary>OPTIONAL: If true, the user can access the browser control's dev tools. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool DevToolsEnabled;

    ///<summary>OPTIONAL: If true, macOS WKWebView is marked inspectable for Safari Web Inspector. Default is false.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool WebInspectorEnabled;

    /// <summary>
    ///     OPTIONAL: If true, native browser control covers the entire screen. Useful for kiosks, for example.
    ///     Incompatible with Maximized and Minimized. Default is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool FullScreen;

    /// <summary>
    ///     OPTIONAL: If true, the native window is maximized to fill the screen. Incompatible with Minimized and FullScreen.
    ///     Default is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Maximized;

    /// <summary>
    ///     OPTIONAL: If true, the native window is minimized (hidden). Incompatible with Maximized and FullScreen. Default is
    ///     false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Minimized;

    /// <summary>
    ///     OPTIONAL: If true, the user cannot resize the native window. Can still be resized by the program. Default
    ///     is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Resizable;

    /// <summary>
    ///     OPTIONAL: If true, a native window appears in front of other windows and cannot be hidden behind them. Default
    ///     is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool Topmost;

    /// <summary>
    ///     OPTIONAL: If true, overrides Top and Left parameters and lets the OS size the newly created window. Default is
    ///     true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool UseOsDefaultLocation;

    /// <summary>
    ///     OPTIONAL: If true, overrides Height and Width parameters and lets the OS size the newly created window.
    ///     Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool UseOsDefaultSize;

    /// <summary>
    ///     OPTIONAL: If true, requests for access to local resources (camera, microphone, etc.) will automatically be
    ///     granted. Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool GrantBrowserPermissions;

    ///<summary>OPTIONAL: If true, browser control allows autoplaying media when a page is loaded. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool MediaAutoplayEnabled;

    ///<summary>OPTIONAL: If true, the browser allows access to the local file system. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool FileSystemAccessEnabled;

    /// <summary>
    ///     OPTIONAL: Determines whether web security features are enabled or disabled.
    ///     Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool WebSecurityEnabled;

    /// <summary>
    ///     OPTIONAL: Enables JavaScript access to the system clipboard when set to true.
    ///     Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool JavascriptClipboardAccessEnabled;

    /// <summary>
    ///     OPTIONAL: Indicates whether media streaming is enabled.
    ///     If set to true, media streaming functionality will be available. Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool MediaStreamEnabled;

    /// <summary>
    ///     OPTIONAL: Enables smooth scrolling behavior if set to true.
    ///     Default value is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool SmoothScrollingEnabled;

    /// <summary>
    ///     OPTIONAL: If true, certificate errors encountered by the browser control will be ignored.
    ///     Typically used to allow navigation to websites with invalid or untrusted certificates.
    ///     Default is false.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool IgnoreCertificateErrorsEnabled;

    ///<summary>OPTIONAL: If true, the status bar (URL hover indicator) is shown. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool StatusBarEnabled;

    ///<summary>OPTIONAL: If true, browser keyboard shortcuts (e.g. Ctrl+T, Ctrl+W, F11) are enabled. Default is true.</summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool BrowserShortcutsEnabled;

    /// <summary>
    ///     WINDOWS: OPTIONAL: If true, toast notifications are allowed on Windows by calling ShowNotification. Requires
    ///     registering the app with Windows, which is not always desirable as it creates shortcuts, etc. Default is true.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    internal bool NotificationsEnabled;

    // ── Background color (RGBA) ────────────────────────────────────────────

    ///<summary>OPTIONAL: Background color of the webview as RGBA bytes. Default is (0, 0, 0, 0).</summary>
    [MarshalAs(UnmanagedType.U1)]
    internal byte BackgroundColorR;

    ///<summary>OPTIONAL: Background color of the webview as RGBA bytes. Default is (0, 0, 0, 0).</summary>
    [MarshalAs(UnmanagedType.U1)]
    internal byte BackgroundColorG;

    ///<summary>OPTIONAL: Background color of the webview as RGBA bytes. Default is (0, 0, 0, 0).</summary>
    [MarshalAs(UnmanagedType.U1)]
    internal byte BackgroundColorB;

    ///<summary>OPTIONAL: Background color of the webview as RGBA bytes. Default is (0, 0, 0, 0).</summary>
    [MarshalAs(UnmanagedType.U1)]
    internal byte BackgroundColorA;

    // ── Menu ───────────────────────────────────────────────────────────────

    /// <summary>
    ///     OPTIONAL: JSON-serialized menu bar structure. Passed to the native layer at window creation.
    /// </summary>
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? MenuBarJson;

    // ── ABI version (must remain last) ─────────────────────────────────────

    /// <summary>
    ///     Set when GetParamErrors() is called before initializing the native window. It is a check to make sure the
    ///     struct matches what C++ is expecting. This field is readonly to ensure ABI stability; do not modify after
    ///     construction.
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    internal readonly int Size = Marshal.SizeOf<InfiniFrameNativeParameters>();
}
