// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IFilePickerDialogsInfiniFrameWindowFeature {
    /// <summary>
    ///     Shows an open file dialog and returns the selected file paths.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
    /// <param name="filters">File extension filters.</param>
    /// <returns>An array of selected file paths or an empty array if canceled.</returns>
    string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null);

    /// <summary>
    ///     Shows an open file dialog asynchronously and returns the selected file paths.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to an array of selected file paths, or an empty array if canceled.</returns>
    Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default);

    /// <summary>
    ///     Shows an open folder dialog and returns the selected folder paths.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
    /// <returns>An array of selected folder paths or an empty array if canceled.</returns>
    string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false);

    /// <summary>
    ///     Shows an open folder dialog asynchronously and returns the selected folder paths.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to an array of selected folder paths, or an empty array if canceled.</returns>
    Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default);

    /// <summary>
    ///     Shows a save file dialog and returns the selected file path.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
    /// <returns>The selected file path, or <c>null</c> if canceled.</returns>
    string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null);

    /// <summary>
    ///     Shows a save file dialog asynchronously and returns the selected file path.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="defaultPath">The default directory path.</param>
    /// <param name="filters">File extension filters.</param>
    /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to the selected file path, or <c>null</c> if canceled.</returns>
    Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null, CancellationToken ct = default);
}