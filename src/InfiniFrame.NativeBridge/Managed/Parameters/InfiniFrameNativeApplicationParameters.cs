// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Parameters;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Process-wide configuration passed to the native InfiniFrame application.
/// Field order is part of the managed/native ABI. Append fields before <see cref="Size" /> only.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct InfiniFrameNativeApplicationParameters() {
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? WebView2RuntimePath;

    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? NotificationRegistrationId;

    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? AppUserModelId;

    [MarshalAs(UnmanagedType.LPUTF8Str)]
    internal string? DefaultNotificationIcon;

    internal readonly int Size = Marshal.SizeOf<InfiniFrameNativeApplicationParameters>();
}
