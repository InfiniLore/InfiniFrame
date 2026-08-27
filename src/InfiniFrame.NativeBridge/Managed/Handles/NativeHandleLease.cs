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

    /// <summary>
    ///     Gets the raw native window handle pointer.
    /// </summary>
    public IntPtr Handle { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Releases the reference to the underlying native handle.
    /// </summary>
    public void Dispose() {
        NativeWindowHandle? handle = Interlocked.Exchange(ref _handle, null);
        handle?.DangerousRelease();
    }
}
