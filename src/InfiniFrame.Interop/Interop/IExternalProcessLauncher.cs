// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Abstraction for launching external processes. Enables testability by allowing substitution
///     of the real <see cref="Process.Start()" /> call with a mock in unit tests.
/// </summary>
public interface IExternalProcessLauncher {
    /// <summary>
    ///     Starts an external process with the specified start information.
    /// </summary>
    /// <param name="startInfo">The process start information.</param>
    /// <returns>The started process, or <c>null</c> if the process was reused.</returns>
    Process? Start(ProcessStartInfo startInfo);
}
