using Microsoft.Win32.SafeHandles;

namespace InfiniFrame.NativeBridge.Handles;

/// <summary>Owns a native InfiniFrame application instance.</summary>
public sealed class NativeApplicationHandle : SafeHandleZeroOrMinusOneIsInvalid {
    internal NativeApplicationHandle(IntPtr handle) : base(true) => SetHandle(handle);

    protected override bool ReleaseHandle() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationDestructor(handle);
        return status == InfiniFrameNativeInteropStatus.Success;
    }
}
