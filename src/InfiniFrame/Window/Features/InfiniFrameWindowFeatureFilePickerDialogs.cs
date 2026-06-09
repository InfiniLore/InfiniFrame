// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureFilePickerDialogs(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureFilePickerDialogs> logger
) : IInfiniFrameWindowFeatureFilePickerDialogs {
    internal const string DefaultFilePickerTitle = "Choose file";
    internal const string DefaultFolderPickerTitle = "Select folder";
    internal const string DefaultSaveFilePickerTitle = "Save file";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    public string?[] ShowOpenFile(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
        => ShowOpenDialog(false, title, defaultPath, multiSelect, filters);
    
    public Task<string?[]> ShowOpenFileAsync(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowOpenFile(title, defaultPath, multiSelect, filters), ct);
    
    public string?[] ShowOpenFolder(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false)
        => ShowOpenDialog(true, title, defaultPath, multiSelect, null);
    
    public Task<string?[]> ShowOpenFolderAsync(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowOpenFolder(title, defaultPath, multiSelect), ct);
    
    public string? ShowSaveFile(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null) {
        if (window.IsClosedOrClosing()) return null;
        
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= [];

        string[] nativeFilters = GetNativeFilters(filters);

        NativeInvoke.InvokeSyncWithValidation<string, string, string[], int, string?, string>(
            logger,
            window.InstanceHandle, 
            window.ManagedThreadId,
            InfiniFrameNative.ShowSaveFile,
            title,
            defaultPath,
            nativeFilters,
            filters.Length,
            null, // TODO actually assign this
            out string? result
        );

        return result;
    }
    
    public Task<string?> ShowSaveFileAsync(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowSaveFile(title, defaultPath, filters), ct);

    private string?[] ShowOpenDialog(bool foldersOnly, string title, string? defaultPath, bool multiSelect, (string Name, string[] Extensions)[]? filters) {
        if (window.IsClosedOrClosing()) return [];
        
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= [];

        string?[]? results;
        string[] nativeFilters = GetNativeFilters(filters, foldersOnly);

        if (foldersOnly) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle, 
                window.ManagedThreadId,
                InfiniFrameNative.ShowOpenFolder,
                title,
                defaultPath,
                multiSelect,
                out results
            );
        }
        else {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.ShowOpenFile,
                title,
                defaultPath,
                multiSelect,
                nativeFilters,
                nativeFilters.Length,
                out results
            );
        }

        return results ?? [];
    }
    
    private static Task<TResult> ShowOpenDialogAsync<TResult>(Func<TResult> workItem, CancellationToken ct = default) =>
        ct.IsCancellationRequested
            ? Task.FromCanceled<TResult>(ct)
            // Dialog calls are intentionally offloaded for Blazor flows where synchronous dialog invocation is unsafe.
            : Task.Run(workItem, ct);
    
    private static string[] GetNativeFilters((string Name, string[] Extensions)[] filters, bool empty = false) {
        string[] nativeFilters = [];
        if (!empty && filters is { Length: > 0 }) {
            nativeFilters = OperatingSystem.IsMacOS()
                ? filters.SelectMany(t => t.Extensions.Select(s => s == "*" ? s : s.TrimStart('*', '.'))).ToArray()
                : filters.Select(t => $"{t.Name}|{t.Extensions.Select(s => s.StartsWith('.') ? $"*{s}" : !s.StartsWith("*.") ? $"*.{s}" : s).Aggregate((e1, e2) => $"{e1};{e2}")}").ToArray();
        }

        return nativeFilters;
    }
}

public static class InfiniFrameWindowFeatureFilePickerDialogsExtensions {
    public static string?[] ShowOpenFile(
        this IInfiniFrameWindow window,
        string title = InfiniFrameWindowFeatureFilePickerDialogs.DefaultFilePickerTitle,
        string? defaultPath = null,
        bool multiSelect = false,
        (string Name, string[] Extensions)[]? filters = null
    )
        => window.Features.FilePickerDialogs.ShowOpenFile(title, defaultPath, multiSelect, filters);
    
    public static Task<string?[]> ShowOpenFileAsync(
        this IInfiniFrameWindow window,
        string title = InfiniFrameWindowFeatureFilePickerDialogs.DefaultFilePickerTitle,
        string? defaultPath = null,
        bool multiSelect = false,
        (string Name, string[] Extensions)[]? filters = null,
        CancellationToken ct = default
    )
        => window.Features.FilePickerDialogs.ShowOpenFileAsync(title, defaultPath, multiSelect, filters, ct);
    
    public static string?[] ShowOpenFolder(
        this IInfiniFrameWindow window,
        string title = InfiniFrameWindowFeatureFilePickerDialogs.DefaultFolderPickerTitle,
        string? defaultPath = null,
        bool multiSelect = false
    )
        => window.Features.FilePickerDialogs.ShowOpenFolder(title, defaultPath, multiSelect);
    
    public static Task<string?[]> ShowOpenFolderAsync(
        this IInfiniFrameWindow window,
        string title = InfiniFrameWindowFeatureFilePickerDialogs.DefaultFilePickerTitle,
        string? defaultPath = null,
        bool multiSelect = false,
        CancellationToken ct = default
    )
        => window.Features.FilePickerDialogs.ShowOpenFolderAsync(title, defaultPath, multiSelect, ct);
    
    public static string? ShowSaveFile(
        this IInfiniFrameWindow window,
        string title = "Save file",
        string? defaultPath = null,
        (string Name, string[] Extensions)[]? filters = null
    ) 
        => window.Features.FilePickerDialogs.ShowSaveFile(title, defaultPath, filters);
    
    public static Task<string?> ShowSaveFileAsync(
        this IInfiniFrameWindow window,
        string title = InfiniFrameWindowFeatureFilePickerDialogs.DefaultSaveFilePickerTitle,
        string? defaultPath = null,
        (string Name, string[] Extensions)[]? filters = null,
        CancellationToken ct = default
    )
        => window.Features.FilePickerDialogs.ShowSaveFileAsync(title, defaultPath, filters, ct);
}