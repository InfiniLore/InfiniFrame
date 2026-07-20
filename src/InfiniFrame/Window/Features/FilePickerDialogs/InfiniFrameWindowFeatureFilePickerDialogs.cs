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

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowOpenFile" />
    public string?[] ShowOpenFile(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
        => ShowOpenDialog(false, title, defaultPath, multiSelect, filters);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowOpenFileAsync" />
    public Task<string?[]> ShowOpenFileAsync(string title = DefaultFilePickerTitle, string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowOpenFile(title, defaultPath, multiSelect, filters), ct);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowOpenFolder" />
    public string?[] ShowOpenFolder(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false)
        => ShowOpenDialog(true, title, defaultPath, multiSelect, null);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowOpenFolderAsync" />
    public Task<string?[]> ShowOpenFolderAsync(string title = DefaultFolderPickerTitle, string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowOpenFolder(title, defaultPath, multiSelect), ct);

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowSaveFile" />
    public string? ShowSaveFile(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null) {
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
            null// TODO actually assign this
        );

        return result;
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureFilePickerDialogs.ShowSaveFileAsync" />
    public Task<string?> ShowSaveFileAsync(string title = DefaultSaveFilePickerTitle, string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => ShowOpenDialogAsync(workItem: () => ShowSaveFile(title, defaultPath, filters), ct);

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

    private static Task<TResult> ShowOpenDialogAsync<TResult>(Func<TResult> workItem, CancellationToken ct = default) {
        ct.ThrowIfCancellationRequested();

        // These platform dialogs are modal APIs with no native completion callback. Running
        // them on the thread pool is both fake asynchrony and invalid for several UI toolkits.
        // Preserve the API while reporting the actual, synchronous platform behavior.
        return Task.FromResult(workItem());
    }

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
