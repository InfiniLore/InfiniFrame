// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using System.Drawing;
using Microsoft.Extensions.Logging;

namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindow : IInfiniFrameWindowBase {
    internal ILogger<IInfiniFrameWindow> Logger { get; }
    IInfiniFrameWindowEvents Events { get; }
    IInfiniFrameWindowMessageHandlers MessageHandlers { get; }

    IntPtr InstanceHandle { get; }
    IntPtr WindowHandle { get; }
    IntPtr NativeType { get; }
    ImmutableArray<Monitor> Monitors { get; }
    Monitor MainMonitor { get; }
    uint ScreenDpi { get; }
    Guid Id { get; }
    Point Location { get; }
    Size MaxSize { get; set; }
    Size MinSize { get; set; }
    Size Size { get; }
    IInfiniFrameWindow? Parent { get; }
    int ManagedThreadId { get; }
    Rectangle CachedPreFullScreenBounds { get; internal set; }
    Rectangle CachedPreMaximizedBounds { get; internal set; }

    void Invoke(Action workItem);
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
    InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info);
    IInfiniFrameWindow RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler);
}
