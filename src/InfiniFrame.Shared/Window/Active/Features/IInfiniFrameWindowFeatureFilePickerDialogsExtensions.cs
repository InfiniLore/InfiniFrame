// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureFilePickerDialogsExtensions {
    public static string?[] ShowOpenFile(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null) 
        => window.Features.FilePickerDialogs.ShowOpenFile(title, defaultPath, multiSelect, filters);
    
    public static Task<string?[]> ShowOpenFileAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default) 
        => window.Features.FilePickerDialogs.ShowOpenFileAsync(title, defaultPath, multiSelect, filters, ct);
    
    public static string?[] ShowOpenFolder(this IInfiniFrameWindow window, string title = "Select folder", string? defaultPath = null, bool multiSelect = false) 
        => window.Features.FilePickerDialogs.ShowOpenFolder(title, defaultPath, multiSelect);
    
    public static Task<string?[]> ShowOpenFolderAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default) 
        => window.Features.FilePickerDialogs.ShowOpenFolderAsync(title, defaultPath, multiSelect, ct);
    
    public static string? ShowSaveFile(this IInfiniFrameWindow window, string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null) 
        => window.Features.FilePickerDialogs.ShowSaveFile(title, defaultPath, filters);
    
    public static Task<string?> ShowSaveFileAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default) 
        => window.Features.FilePickerDialogs.ShowSaveFileAsync(title, defaultPath, filters, ct);
}
