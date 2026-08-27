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

    protected override bool ReleaseHandle() {
        InfiniFrameNative.Destructor(handle);

        // Always return true to prevent SafeHandle finalizer from retrying a doomed destructor.
        // Logging is not available in finalizer context; the destructor status is observable
        // via the window lifecycle state if needed.
        return true;
    }
}
