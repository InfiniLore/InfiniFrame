// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IFilePickerDialogsInfiniFrameWindowFeature" /> on
///     <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IFilePickerDialogsInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Shows an open file dialog and returns the selected file paths.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
        /// <param name="filters">File extension filters.</param>
        /// <returns>An array of selected file paths or an empty array if canceled.</returns>
        public string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
            => window.Features.FilePickerDialogs.ShowOpenFile(title, defaultPath, multiSelect, filters);
        /// <summary>
        ///     Shows an open file dialog asynchronously and returns the selected file paths.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="multiSelect">Whether multiple file selection is allowed.</param>
        /// <param name="filters">File extension filters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that resolves to an array of selected file paths, or an empty array if canceled.</returns>
        public Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
            => window.Features.FilePickerDialogs.ShowOpenFileAsync(title, defaultPath, multiSelect, filters, ct);
        /// <summary>
        ///     Shows an open folder dialog and returns the selected folder paths.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
        /// <returns>An array of selected folder paths or an empty array if canceled.</returns>
        public string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false)
            => window.Features.FilePickerDialogs.ShowOpenFolder(title, defaultPath, multiSelect);
        /// <summary>
        ///     Shows an open folder dialog asynchronously and returns the selected folder paths.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="multiSelect">Whether multiple folder selection is allowed.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that resolves to an array of selected folder paths, or an empty array if canceled.</returns>
        public Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
            => window.Features.FilePickerDialogs.ShowOpenFolderAsync(title, defaultPath, multiSelect, ct);
        /// <summary>
        ///     Shows a save file dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="filters">File extension filters.</param>
        /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
        /// <returns>The selected file path, or <c>null</c> if canceled.</returns>
        public string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null)
            => window.Features.FilePickerDialogs.ShowSaveFile(title, defaultPath, filters, defaultFileName);
        /// <summary>
        ///     Shows a save file dialog asynchronously and returns the selected file path.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultPath">The default directory path.</param>
        /// <param name="filters">File extension filters.</param>
        /// <param name="defaultFileName">The default file name to pre-populate in the save dialog.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that resolves to the selected file path, or <c>null</c> if canceled.</returns>
        public Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, string? defaultFileName = null, CancellationToken ct = default)
            => window.Features.FilePickerDialogs.ShowSaveFileAsync(title, defaultPath, filters, defaultFileName, ct);
    }

}
