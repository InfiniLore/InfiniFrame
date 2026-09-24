using Microsoft.Win32.SafeHandles;

namespace InfiniFrame.NativeBridge.Handles;

/// <summary>Owns a native InfiniFrame application instance.</summary>
public sealed class NativeApplicationHandle : SafeHandleZeroOrMinusOneIsInvalid {
    private int _releaseStatus = (int)InfiniFrameNativeInteropStatus.Success;

    internal NativeApplicationHandle(IntPtr handle) : base(true) => SetHandle(handle);

    internal InfiniFrameNativeInteropStatus ReleaseStatus
        => (InfiniFrameNativeInteropStatus)Volatile.Read(ref _releaseStatus);

    internal bool TryRelease() {
        if (IsClosed) return ReleaseStatus == InfiniFrameNativeInteropStatus.Success;

        bool released = ReleaseHandle();
        if (released) SetHandleAsInvalid();
        return released;
    }

    protected override bool ReleaseHandle() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationDestructor(handle);
        Volatile.Write(ref _releaseStatus, (int)status);
        return status == InfiniFrameNativeInteropStatus.Success;
    }
}
