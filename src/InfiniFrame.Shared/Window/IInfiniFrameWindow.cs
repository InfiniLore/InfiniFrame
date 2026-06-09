// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Dialogs;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindow : IHasInfiniFrameProperties, IHasInfiniFrameEventsStore {
    IServiceProvider? ServiceProvider { get; }
    IInfiniFrameEvents Events { get; }
    IInfiniFrameWindowDebugging Debugging { get; }
    IInfiniFrameWindowFeatures Features { get; }
    
    IInfiniFrameOptions Configuration { get; }
    
    IntPtr InstanceHandle { get; internal set; }

    IntPtr WindowHandle { get; }
    IntPtr NativeType { get; }
    uint ScreenDpi { get; }
    Guid Id { get; }
    Point Location { get; }
    Size MaxSize { get; }
    Size MinSize { get; }
    Size Size { get; }
    int ManagedThreadId { get; }
    Rectangle CachedPreFullScreenBounds { get; internal set; }
    Rectangle CachedPreMaximizedBounds { get; internal set; }
    bool Focused { get; }
    
    string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null);
    Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default);
    string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false);
    Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default);
    string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null);
    Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default);
   
    internal void MarkClosedFromNativeCallback();
}
