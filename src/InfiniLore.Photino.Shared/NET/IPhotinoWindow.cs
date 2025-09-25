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
    IPhotinoWindowEvents Events { get; }
    
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
    PhotinoDialogResult ShowMessage(string title, string? text, PhotinoDialogButtons buttons = PhotinoDialogButtons.Ok, PhotinoDialogIcon icon = PhotinoDialogIcon.Info);
    IPhotinoWindow RegisterCustomSchemeHandler(string scheme, NetCustomSchemeDelegate handler);
}
