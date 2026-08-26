// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Manages the lifecycle of an InfiniFrame window, including initialization, closing, and cleanup.
/// </summary>
public interface ILifecycleInfiniFrameWindowFeature {
    /// <summary>Gets the current deterministic lifecycle state.</summary>
    InfiniFrameWindowLifecycleState State { get; }

    /// <summary>
    ///     Waits asynchronously for the window to reach the ready state.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the window is ready or cancellation is requested.</returns>
    ValueTask WaitForReadyAsync(CancellationToken ct = default);

    /// <summary>
    ///     Initializes the window lifecycle, performing native window creation and setup.
    /// </summary>
    internal void Initialize();

    /// <summary>
    ///     Blocks the calling thread until the window is closed.
    /// </summary>
    void WaitForClose();

    /// <summary>
    ///     Waits asynchronously for the window to close.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when the window closes or cancellation is requested.</returns>
    ValueTask WaitForCloseAsync(CancellationToken ct = default);

    /// <summary>
    ///     Waits asynchronously for all registered close callbacks to complete.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when all close callbacks have been delivered or cancellation is requested.</returns>
    ValueTask WaitForClosedCallbacksAsync(CancellationToken ct = default);

    /// <summary>
    ///     Waits asynchronously for the window teardown to complete, including native handle release.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the wait operation.</param>
    /// <returns>A task that completes when teardown is finished or cancellation is requested.</returns>
    ValueTask WaitForTeardownAsync(CancellationToken ct = default);

    /// <summary>
    ///     Closes the window synchronously.
    /// </summary>
    void Close();

    /// <summary>
    ///     Closes the window asynchronously.
    /// </summary>
    /// <param name="ct">A cancellation token to cancel the close operation.</param>
    /// <returns>A task that completes when the window close operation finishes.</returns>
    ValueTask CloseAsync(CancellationToken ct = default);

    /// <summary>
    ///     Marks the window as closed without performing native cleanup.
    /// </summary>
    internal void MarkAsClosed();
    internal void MarkClosedCallbacksDelivered();
    internal void MarkCloseRejected();

    /// <summary>
    ///     Frees the native window handle. Must be called outside of native signal handlers.
    /// </summary>
    internal void CleanupNativeHandle();

    internal bool CanWaitForCloseDuringDispose();
    internal bool CanWaitForTeardownDuringDispose();

    /// <summary>
    ///     Checks whether the window is closed or in the process of closing.
    /// </summary>
    /// <returns><c>true</c> if the window is closed or closing; otherwise, <c>false</c>.</returns>
    bool IsClosedOrClosing();
}
