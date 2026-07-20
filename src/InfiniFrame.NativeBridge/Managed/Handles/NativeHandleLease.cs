// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge.Handles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Keeps a safe handle alive for the complete duration of a native operation.
/// </summary>
public sealed class NativeHandleLease : IDisposable {
    private NativeWindowHandle? _handle;

    public IntPtr Handle { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal NativeHandleLease(NativeWindowHandle handle) {
        bool addedRef = false;
        try {
            handle.DangerousAddRef(ref addedRef);
            ObjectDisposedException.ThrowIf(!addedRef || handle.IsInvalid || handle.IsClosed, nameof(NativeWindowHandle));

            _handle = handle;
            Handle = handle.DangerousGetHandle();
        }
        catch {
            if (addedRef) handle.DangerousRelease();
            throw;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        NativeWindowHandle? handle = Interlocked.Exchange(ref _handle, null);
        handle?.DangerousRelease();
    }
}
