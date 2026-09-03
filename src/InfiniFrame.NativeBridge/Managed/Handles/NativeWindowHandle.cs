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
    private volatile bool _safeToDestroy;
    private int _released;

    internal NativeWindowHandle(IntPtr handle) : base(true) {
        SetHandle(handle);
    }

    internal NativeWindowHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle) {
        SetHandle(handle);
    }

    internal void MarkSafeToDestroy() {
        _safeToDestroy = true;
        TryDestroy();
    }

    protected override bool ReleaseHandle() {
        TryDestroy();

        // Always return true to prevent SafeHandle finalizer from retrying a doomed destructor.
        // Logging is not available in finalizer context; the destructor status is observable
        // via the window lifecycle state if needed.
        return true;
    }

    private void TryDestroy() {
        if (_safeToDestroy && Interlocked.CompareExchange(ref _released, 1, 0) == 0) {
            InfiniFrameNative.Destructor(handle);
        }
    }
}
