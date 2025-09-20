// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniLore.Photino.NET;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPhotinoWindow : IPhotinoWindowBase
{
    IntPtr InstanceHandle { get; }
    IntPtr WindowHandle { get; }
    IReadOnlyList<Monitor> Monitors { get; }
    Monitor MainMonitor { get; }
    uint ScreenDpi { get; }
    Guid Id { get; }
    string? IconFilePath { get; }
    Point Location { get; }
    int Left { get; }
    bool Maximized { get; }
    Point MaxSize { get; }
    int MaxHeight { get; }
    int MaxWidth { get; }
    bool Minimized { get; }
    Point MinSize { get; }
    int MinHeight { get; }
    int MinWidth { get; }
    IPhotinoWindow? Parent { get; } 
    bool Resizable { get; }
    Size Size { get; }
    string BrowserControlInitParameters { get; }
    string StartString { get; }
    string StartUrl { get; set; }
    string? TemporaryFilesPath { get; }
    string NotificationRegistrationId { get; }
    string? Title { get; }
    int Top { get; }
    bool Topmost { get; }
    bool UseOsDefaultLocation { get; }
    bool UseOsDefaultSize { get; }
    EventHandler<string>? WebMessageReceivedHandler { get; }
    int Width { get; }
    int ManagedThreadId { get; }

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
    IPhotinoWindow Invoke(Action workItem);
    IPhotinoWindow Load(Uri uri);
    IPhotinoWindow Load(string path);
    IPhotinoWindow LoadRawString(string content);
    IPhotinoWindow Center();
    IPhotinoWindow MoveTo(Point location, bool allowOutsideWorkArea = false);
    IPhotinoWindow MoveTo(int left, int top, bool allowOutsideWorkArea = false);
    IPhotinoWindow Offset(Point offset);
    IPhotinoWindow Offset(int left, int top);
    IPhotinoWindow SetChromeless(bool chromeless);
    IPhotinoWindow SetTransparent(bool enabled);
    IPhotinoWindow SetContextMenuEnabled(bool enabled);
    IPhotinoWindow SetDevToolsEnabled(bool enabled);
    IPhotinoWindow SetFullScreen(bool fullScreen);
    IPhotinoWindow SetBrowserControlInitParameters(string parameters);
    IPhotinoWindow SetNotificationRegistrationId(string notificationRegistrationId);
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
    IPhotinoWindow SetTemporaryFilesPath(string tempFilesPath);
    IPhotinoWindow SetTitle(string title);
    IPhotinoWindow SetTop(int top);
    IPhotinoWindow SetTopMost(bool topMost);
    IPhotinoWindow SetWidth(int width);
    IPhotinoWindow SetZoom(int zoom);
    IPhotinoWindow SetUseOsDefaultLocation(bool useOsDefault);
    IPhotinoWindow SetUseOsDefaultSize(bool useOsDefault);
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
