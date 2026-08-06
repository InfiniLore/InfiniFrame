// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Describes the taskbar capabilities supported by the current platform.
/// </summary>
public sealed record InfiniFrameTaskbarCapabilities {
    /// <summary>
    ///     Gets whether the platform supports taskbar progress indicators.
    /// </summary>
    public required bool SupportsProgress { get; init; }

    /// <summary>
    ///     Gets whether the platform supports taskbar icon flashing.
    /// </summary>
    public required bool SupportsFlash { get; init; }
}
