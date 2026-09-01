// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Safe handle for a native InfiniFrameApplication instance.
/// </summary>
internal sealed class NativeApplicationHandle : SafeHandleZeroOrMinusOneIsInvalid {
    internal NativeApplicationHandle(IntPtr handle) : base(ownsHandle: true) {
        SetHandle(handle);
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationDestructor(handle);
        return status == InfiniFrameNativeInteropStatus.Success;
    }
}
