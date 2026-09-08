// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;
using System.Security;
using System.Security.Principal;
using InfiniFrame.Utilities;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides instance arbitration logic for single-instance enforcement using named mutexes
///     and process elevation detection.
/// </summary>
public static class InstanceArbitration {
    private const string DefaultMutexName = "InfiniFrame.SingleInstance";

    /// <summary>
    ///     Holds the primary instance mutex for the process lifetime.
    ///     The OS reclaims this mutex when the process terminates.
    /// </summary>
    private static volatile Mutex? _primaryMutex;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Attempts to acquire the primary instance lock using a named mutex.
    ///     The mutex is held for the process lifetime and released when the process exits.
    /// </summary>
    /// <param name="mutexName">
    ///     The mutex name to use. If <c>null</c>, the default name <c>InfiniFrame.SingleInstance</c> is used.
    /// </param>
    /// <returns><c>true</c> if this instance is the primary; <c>false</c> if another instance holds the lock.</returns>
    public static bool TryAcquirePrimaryInstance(string? mutexName) {
        string name = mutexName ?? DefaultMutexName;

        try {
            _primaryMutex = new Mutex(true, name, out bool createdNew);

            if (createdNew) return true;

            // Another instance already holds this mutex.
            _primaryMutex.Dispose();
            _primaryMutex = null;
            return false;

        }
        catch (AbandonedMutexException) {
            // A previous instance crashed without releasing the mutex.
            // We now own the mutex (it was abandoned, not released).
            return true;
        }
    }

    /// <summary>
    ///     Determines whether the current process is running with elevated (administrator) privileges.
    /// </summary>
    /// <returns><c>true</c> if the process is elevated; <c>false</c> otherwise.</returns>
    public static bool IsProcessElevated() {
        if (OperatingSystem.IsWindows()) return IsProcessElevatedWindows();
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) return IsProcessElevatedUnix();

        return false;
    }

    [SupportedOSPlatform("windows")]
    private static bool IsProcessElevatedWindows() {
        try {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (SecurityException) {
            return false;
        }
    }

    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("osx")]
    private static bool IsProcessElevatedUnix() => UnixPInvoke.GetUid() == 0;
}
