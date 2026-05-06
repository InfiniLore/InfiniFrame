// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindow : IHasInfiniFrameProperties, IHasInfiniFrameWindowEventsStore {
    ILogger<IInfiniFrameWindow> Logger { get; }
    IServiceProvider? ServiceProvider { get; }
    IInfiniFrameWindowEvents Events { get; }
    
    IInfiniFrameOptions Configuration { get; }
    
    IntPtr InstanceHandle { get; }
    IntPtr WindowHandle { get; }
    IntPtr NativeType { get; }
    ImmutableArray<InfiniMonitor> Monitors { get; }
    InfiniMonitor MainMonitor { get; }
    uint ScreenDpi { get; }
    Guid Id { get; }
    Point Location { get; }
    Size MaxSize { get; }
    Size MinSize { get; }
    Size Size { get; }
    IInfiniFrameWindow? Parent { get; }
    int ManagedThreadId { get; }
    Rectangle CachedPreFullScreenBounds { get; internal set; }
    Rectangle CachedPreMaximizedBounds { get; internal set; }
    bool Focused { get; }

    void Invoke(Action workItem);
    
    void WaitForClose();
    Task WaitForCloseAsync(CancellationToken ct = default);
    
    void Close();
    Task CloseAsync(CancellationToken ct = default);
    
    void SendWebMessage(string message);
    Task SendWebMessageAsync(string message, CancellationToken ct = default);
    void SendNotification(string title, string body);
    string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null);
    Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default);
    string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false);
    Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default);
    string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null);
    Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default);
    InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info);

    bool TryResolveStaticAssetUri(string path, out Uri uri);
    internal void MarkClosedFromNativeCallback();
}
