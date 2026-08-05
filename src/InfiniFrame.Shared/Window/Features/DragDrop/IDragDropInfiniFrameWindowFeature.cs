// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides drag and drop functionality for the window.
/// </summary>
public interface IDragDropInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets whether drag and drop is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    ///     Enables or disables drag and drop.
    /// </summary>
    /// <param name="enabled">Whether to enable drag and drop.</param>
    void SetEnabled(bool enabled);

    /// <summary>
    ///     Gets the allowed file extensions for drop operations.
    ///     Empty means all file types are allowed.
    /// </summary>
    IReadOnlyList<string> AllowedExtensions { get; }

    /// <summary>
    ///     Sets the allowed file extensions for drop operations.
    /// </summary>
    /// <param name="extensions">File extensions (e.g., ".txt", ".png"). Empty to allow all.</param>
    void SetAllowedExtensions(IReadOnlyList<string> extensions);
}
