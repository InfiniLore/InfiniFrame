// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
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

    protected override bool ReleaseHandle() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.Destructor(handle);
        if (status != InfiniFrameNativeInteropStatus.Success) {
            // TODO: Replace with ILogger when available in SafeHandle finalizer context.
            Debug.WriteLine($"[InfiniFrame] Native window destructor failed with status {status}. Handle: {handle}");
        }

        // Always return true to prevent SafeHandle finalizer from retrying a doomed destructor.
        return true;
    }
}
