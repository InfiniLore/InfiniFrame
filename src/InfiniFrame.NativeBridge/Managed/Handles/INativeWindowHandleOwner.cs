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
    NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature);
}