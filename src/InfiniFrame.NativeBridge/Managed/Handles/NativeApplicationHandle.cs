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
    private NativeApplicationHandle() : base(ownsHandle: true) { }

    /// <inheritdoc />
    protected override bool ReleaseHandle() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationDestructor(handle);
        return status == InfiniFrameNativeInteropStatus.Success;
    }
}
