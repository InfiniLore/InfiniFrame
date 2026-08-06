// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines the visual state of the taskbar progress indicator.
///     Maps to the Windows <c>TBPFLAG</c> enum.
/// </summary>
public enum TaskbarProgressState : int {
    /// <summary>
    ///     No progress indicator is displayed.
    /// </summary>
    None = 0,
    /// <summary>
    ///     The progress indicator shows an indeterminate (marquee) animation.
    /// </summary>
    Indeterminate = 1,
    /// <summary>
    ///     The progress indicator shows a normal progress bar.
    /// </summary>
    Normal = 2,
    /// <summary>
    ///     The progress indicator shows an error state (red).
    /// </summary>
    Error = 3,
    /// <summary>
    ///     The progress indicator shows a paused state (yellow).
    /// </summary>
    Paused = 4
}
