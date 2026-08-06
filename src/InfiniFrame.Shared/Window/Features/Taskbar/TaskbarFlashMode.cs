// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines how the taskbar icon should flash.
///     Maps to the Windows <c>FLASHW_*</c> flags.
/// </summary>
public enum TaskbarFlashMode {
    /// <summary>
    ///     Stop flashing.
    /// </summary>
    Stop = 0,
    /// <summary>
    ///     Flash the window until it receives focus.
    /// </summary>
    All = 1,
    /// <summary>
    ///     Flash the window for the specified number of times.
    /// </summary>
    Timer = 2,
    /// <summary>
    ///     Flash the window for the specified number of times until it receives focus.
    /// </summary>
    TimerAll = 3
}
