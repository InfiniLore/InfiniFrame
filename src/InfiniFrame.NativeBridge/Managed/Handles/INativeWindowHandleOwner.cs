// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Implemented by the managed owner that validates lifecycle and grants native leases.
/// </summary>
public interface INativeWindowHandleOwner {
    /// <summary>
    ///     Acquires a <see cref="NativeHandleLease"/> for the duration of a native operation,
    ///     validating the current lifecycle state against the requested access level.
    /// </summary>
    /// <param name="access">The type of access required by the caller.</param>
    /// <returns>A lease that keeps the native handle alive until disposed.</returns>
    NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature);
}
