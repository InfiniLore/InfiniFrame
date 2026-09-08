// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature implementation for showing native file and folder open/save dialogs, including synchronous and
///     asynchronous variants with cancellation support.
/// </summary>
public class FilePickerDialogsInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<FilePickerDialogsInfiniFrameWindowFeature> logger
) : IFilePickerDialogsInfiniFrameWindowFeature {
    internal const string DefaultFilePickerTitle = "Choose file";
    internal const string DefaultFolderPickerTitle = "Select folder";
    internal const string DefaultSaveFilePickerTitle = "Save file";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowOpenFile" />
    public string?[] ShowOpenFile(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
        => ShowOpenDialog(false, title, defaultPath, multiSelect, filters);

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowOpenFileAsync" />
    public Task<string?[]> ShowOpenFileAsync(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => ShowDialogAsync(InfiniFileDialogKind.OpenFile, title, defaultPath, multiSelect, filters, null, ct);

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowOpenFolder" />
    public string?[] ShowOpenFolder(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false)
        => ShowOpenDialog(true, title, defaultPath, multiSelect, null);

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowOpenFolderAsync" />
    public Task<string?[]> ShowOpenFolderAsync(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
        => ShowDialogAsync(InfiniFileDialogKind.OpenFolder, title, defaultPath, multiSelect, null, null, ct);

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowSaveFile" />
    public string? ShowSaveFile(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null) {
        if (window.IsClosedOrClosing()) return null;

        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= [];

        string[] nativeFilters = GetNativeFilters(filters);

        string? result = NativeInvoke.InvokeSyncWithValidation<string, string, string[], int, string?, string>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.ShowSaveFile,
            title,
            defaultPath,
            nativeFilters,
            filters.Length,
            defaultFileName
        );

        return result;
    }

    /// <inheritdoc cref="IFilePickerDialogsInfiniFrameWindowFeature.ShowSaveFileAsync" />
    public Task<string?> ShowSaveFileAsync(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null, CancellationToken ct = default)
        => ShowSaveDialogAsync(title, defaultPath, filters, defaultFileName, ct);

    private string?[] ShowOpenDialog(bool foldersOnly, string title, string? defaultPath, bool multiSelect, (string Name, string[] Extensions)[]? filters) {
        if (window.IsClosedOrClosing()) return [];

        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= [];

        string?[]? results;
        string[] nativeFilters = GetNativeFilters(filters, foldersOnly);

        if (foldersOnly) {
            results = NativeInvoke.InvokeSyncWithValidation<string, string, bool, string?[]>(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.ShowOpenFolder,
                title,
                defaultPath,
                multiSelect
            );
        }
        else {
            results = NativeInvoke.InvokeSyncWithValidation<string, string, bool, string[], int, string?[]>(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.ShowOpenFile,
                title,
                defaultPath,
                multiSelect,
                nativeFilters,
                nativeFilters.Length
            );
        }

        return results ?? [];
    }

    private async Task<string?[]> ShowDialogAsync(
        InfiniFileDialogKind kind,
        string title,
        string? defaultPath,
        bool multiSelect,
        (string Name, string[] Extensions)[]? filters,
        string? defaultFileName,
        CancellationToken ct
    ) {
        ct.ThrowIfCancellationRequested();
        if (window.IsClosedOrClosing()) return [];

        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= [];
        var operation = new InfiniFileDialogOperation(
            window, logger, kind, title, defaultPath, multiSelect,
            GetNativeFilters(filters, kind == InfiniFileDialogKind.OpenFolder), defaultFileName, ct
        );
        _ = operation.StartAsync();
        return await operation.Task.WaitAsync(ct).ConfigureAwait(false);
    }

    private async Task<string?> ShowSaveDialogAsync(
        string title,
        string? defaultPath,
        (string Name, string[] Extensions)[]? filters,
        string? defaultFileName,
        CancellationToken ct
    ) {
        string?[] values = await ShowDialogAsync(
            InfiniFileDialogKind.SaveFile, title, defaultPath, false, filters, defaultFileName, ct
        ).ConfigureAwait(false);
        return values.FirstOrDefault();
    }

    private static string[] GetNativeFilters((string Name, string[] Extensions)[] filters, bool empty = false) {
        string[] nativeFilters = [];
        if (!empty && filters is { Length: > 0 }) {
            nativeFilters = OperatingSystem.IsMacOS()
                ? [.. filters.SelectMany(t => t.Extensions.Select(s => s == "*" ? s : s.TrimStart('*', '.')))]
                : [.. filters.Select(t => $"{t.Name}|{t.Extensions.Select(s => s.StartsWith('.') ? $"*{s}" : !s.StartsWith("*.") ? $"*.{s}" : s).Aggregate((e1, e2) => $"{e1};{e2}")}")];
        }

        return nativeFilters;
    }
}
