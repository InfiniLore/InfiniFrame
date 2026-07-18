// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Resolves non-owning interop pointer values back to their managed SafeHandle owner.</summary>
public static class NativeWindowHandleRegistry {
    private static readonly ConcurrentDictionary<IntPtr, INativeWindowHandleOwner> Owners = new();

    public static void Register(IntPtr handle, INativeWindowHandleOwner owner) {
        ArgumentOutOfRangeException.ThrowIfZero(handle);
        ArgumentNullException.ThrowIfNull(owner);
        if (!Owners.TryAdd(handle, owner))
            throw new InvalidOperationException("The native window handle is already registered.");
    }

    public static void Unregister(IntPtr handle, INativeWindowHandleOwner owner) {
        if (handle != IntPtr.Zero)
            Owners.TryRemove(new KeyValuePair<IntPtr, INativeWindowHandleOwner>(handle, owner));
    }

    public static NativeHandleLease Acquire(IntPtr handle, NativeHandleAccess access = NativeHandleAccess.Feature) {
        if (handle == IntPtr.Zero || !Owners.TryGetValue(handle, out INativeWindowHandleOwner? owner))
            throw new ObjectDisposedException("InfiniFrameWindow", "The native window is unavailable.");

        return owner.AcquireNativeHandle(access);
    }
}
