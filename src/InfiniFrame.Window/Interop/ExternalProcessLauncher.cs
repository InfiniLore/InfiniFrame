// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Window.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Default implementation that delegates to <see cref="Process.Start(ProcessStartInfo)" />.
/// </summary>
internal sealed class ExternalProcessLauncher : IExternalProcessLauncher {
    public Process? Start(ProcessStartInfo startInfo) => Process.Start(startInfo);
}
