// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Win32.SafeHandles;

namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Owns a native InfiniFrame window instance.</summary>
public sealed class NativeWindowHandle : SafeHandleZeroOrMinusOneIsInvalid {
    internal NativeWindowHandle(IntPtr handle) : base(true) {
        SetHandle(handle);
    }

    internal NativeWindowHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
        => InfiniFrameNative.Destructor(handle) == InfiniFrameNativeInteropStatus.Success;
}