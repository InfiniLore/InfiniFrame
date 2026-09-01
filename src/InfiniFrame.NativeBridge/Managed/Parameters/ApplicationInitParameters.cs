// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the parameters used to configure and initialize a native InfiniFrame application.
///     Passed to the native layer as a sequentially laid-out struct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ApplicationInitParameters {
    /// <summary>
    ///     The size of this struct. Used for ABI version checking.
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public int StructSize;

    // ── Process identity (Win32) ──────────────────────────────────────────

    /// <summary>
    ///     WINDOWS ONLY: OPTIONAL: Explicit application identity used by the taskbar for grouping and pinning.
    /// </summary>
    public IntPtr WindowsAppUserModelId;

    /// <summary>
    ///     WINDOWS ONLY: OPTIONAL: Registers the application for toast notifications.
    /// </summary>
    public IntPtr NotificationRegistrationId;

    // ── WebView2 runtime path override (Win32) ────────────────────────────

    /// <summary>
    ///     WINDOWS ONLY: OPTIONAL: Path to an extracted fixed-version WebView2 runtime.
    /// </summary>
    public IntPtr WebView2RuntimePath;

    // ── ABI version (must remain last) ────────────────────────────────────

    /// <summary>
    ///     Reserved for future use.
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public int Reserved;
}
