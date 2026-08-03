// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IFilePickerDialogsInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Shows an open file dialog and returns the selected file paths.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
    /// <param name="filters">File extension filters.</param>
    /// <returns>An array of selected file paths, or an empty array if cancelled.</returns>
    public static string?[] ShowOpenFile(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
        => window.Features.FilePickerDialogs.ShowOpenFile(title, defaultPath, multiSelect, filters);

    /// <summary>
    ///     Shows an open file dialog asynchronously and returns the selected file paths.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to an array of selected file paths, or an empty array if cancelled.</returns>
    public static Task<string?[]> ShowOpenFileAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => window.Features.FilePickerDialogs.ShowOpenFileAsync(title, defaultPath, multiSelect, filters, ct);

    /// <summary>
    ///     Shows an open folder dialog and returns the selected folder paths.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
    /// <returns>An array of selected folder paths, or an empty array if cancelled.</returns>
    public static string?[] ShowOpenFolder(this IInfiniFrameWindow window, string title = "Select folder", string? defaultPath = null, bool multiSelect = false)
        => window.Features.FilePickerDialogs.ShowOpenFolder(title, defaultPath, multiSelect);

    /// <summary>
    ///     Shows an open folder dialog asynchronously and returns the selected folder paths.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to an array of selected folder paths, or an empty array if cancelled.</returns>
    public static Task<string?[]> ShowOpenFolderAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
        => window.Features.FilePickerDialogs.ShowOpenFolderAsync(title, defaultPath, multiSelect, ct);

    /// <summary>
    ///     Shows a save file dialog and returns the selected file path.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
    /// <returns>The selected file path, or <c>null</c> if cancelled.</returns>
    public static string? ShowSaveFile(this IInfiniFrameWindow window, string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null)
        => window.Features.FilePickerDialogs.ShowSaveFile(title, defaultPath, filters, defaultFileName);

    /// <summary>
    ///     Shows a save file dialog asynchronously and returns the selected file path.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to the selected file path, or <c>null</c> if cancelled.</returns>
    public static Task<string?> ShowSaveFileAsync(this IInfiniFrameWindow window, string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null, CancellationToken ct = default)
        => window.Features.FilePickerDialogs.ShowSaveFileAsync(title, defaultPath, filters, defaultFileName, ct);
}