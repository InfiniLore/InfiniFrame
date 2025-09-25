// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniLore.Photino.NET;
using Microsoft.Extensions.Logging;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPhotinoWindow : IPhotinoWindowBase {
    internal ILogger<IPhotinoWindow> Logger { get; }
    
    IntPtr InstanceHandle { get; }
    IntPtr WindowHandle { get; }
    ImmutableArray<Monitor> Monitors { get; }
    Monitor MainMonitor { get; }
    uint ScreenDpi { get; }
    Guid Id { get; }
    Point Location { get; }
    Point MaxSize { get; }
    Point MinSize { get; }
    Size Size { get; }
    IPhotinoWindow? Parent { get; }
    int ManagedThreadId { get; }
    Rectangle CachedPreFullScreenBounds { get; internal set; }

    event EventHandler<Point>? WindowLocationChanged;
    event EventHandler<Size>? WindowSizeChanged;
    event EventHandler? WindowFocusIn;
    event EventHandler? WindowMaximized;
    event EventHandler? WindowRestored;
    event EventHandler? WindowFocusOut;
    event EventHandler? WindowMinimized;
    event EventHandler<string>? WebMessageReceived;
    event NetClosingDelegate? WindowClosing;
    event EventHandler? WindowCreating;
    event EventHandler? WindowCreated;

    int Zoom { get; }
    void Invoke(Action workItem);
    IPhotinoWindow Offset(Point offset);
    IPhotinoWindow Offset(int left, int top);
    IPhotinoWindow SetChromeless(bool chromeless);
    IPhotinoWindow SetTransparent(bool enabled);
    IPhotinoWindow SetContextMenuEnabled(bool enabled);
    IPhotinoWindow SetDevToolsEnabled(bool enabled);
    IPhotinoWindow SetFullScreen(bool fullScreen);
    IPhotinoWindow SetHeight(int height);
    IPhotinoWindow SetIconFile(string iconFile);
    IPhotinoWindow SetLeft(int left);
    IPhotinoWindow SetResizable(bool resizable);
    IPhotinoWindow SetSize(Size size);
    IPhotinoWindow SetSize(int width, int height);
    IPhotinoWindow SetLocation(Point location);
    IPhotinoWindow SetMaximized(bool maximized);
    IPhotinoWindow SetMaxSize(int maxWidth, int maxHeight);
    IPhotinoWindow SetMaxHeight(int maxHeight);
    IPhotinoWindow SetMaxWidth(int maxWidth);
    IPhotinoWindow SetMinimized(bool minimized);
    IPhotinoWindow SetMinSize(int minWidth, int minHeight);
    IPhotinoWindow SetMinHeight(int minHeight);
    IPhotinoWindow SetMinWidth(int minWidth);
    IPhotinoWindow SetTitle(string? title);
    IPhotinoWindow SetTop(int top);
    IPhotinoWindow SetTopMost(bool topMost);
    IPhotinoWindow SetWidth(int width);
    IPhotinoWindow SetZoom(int zoom);
    IPhotinoWindow Win32SetWebView2Path(string data);
    IPhotinoWindow ClearBrowserAutoFill();
    void WaitForClose();
    void Close();
    void SendWebMessage(string message);
    Task SendWebMessageAsync(string message);
    void SendNotification(string title, string body);
    string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null);
    Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null);
    string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false);
    Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false);
    string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null);
    Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null);
    PhotinoDialogResult ShowMessage(string title, string? text, PhotinoDialogButtons buttons = PhotinoDialogButtons.Ok, PhotinoDialogIcon icon = PhotinoDialogIcon.Info);
    IPhotinoWindow RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler);
}
