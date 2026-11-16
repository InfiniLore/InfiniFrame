// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="ILifecycleInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />
///     .
/// </summary>
public static class ILifecycleInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Waits until the window is ready for interaction.
        /// </summary>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        public ValueTask WaitForReadyAsync(CancellationToken ct = default)
            => window.Features.Lifecycle.WaitForReadyAsync(ct);
        /// <summary>
        ///     Blocks the calling thread until the window is closed.
        /// </summary>
        public void WaitForClose()
            => window.Features.Lifecycle.WaitForClose();
        /// <summary>
        ///     Asynchronously waits until the window is closed.
        /// </summary>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        public ValueTask WaitForCloseAsync(CancellationToken ct = default)
            => window.Features.Lifecycle.WaitForCloseAsync(ct);
        /// <summary>
        ///     Closes the window synchronously.
        /// </summary>
        public void Close()
            => window.Features.Lifecycle.Close();
        /// <summary>
        ///     Closes the window asynchronously.
        /// </summary>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        public ValueTask CloseAsync(CancellationToken ct = default)
            => window.Features.Lifecycle.CloseAsync(ct);
        /// <summary>
        ///     Waits for closed callbacks to complete.
        /// </summary>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        public ValueTask WaitForClosedCallbacksAsync(CancellationToken ct = default)
            => window.Features.Lifecycle.WaitForClosedCallbacksAsync(ct);
        /// <summary>
        ///     Waits for the window teardown to complete.
        /// </summary>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        public ValueTask WaitForTeardownAsync(CancellationToken ct = default)
            => window.Features.Lifecycle.WaitForTeardownAsync(ct);
        /// <summary>
        ///     Returns whether the window is closed or in the closing process.
        /// </summary>
        /// <returns><c>true</c> if the window is closed or closing; otherwise, <c>false</c>.</returns>
        public bool IsClosedOrClosing()
            => window.Features.Lifecycle.IsClosedOrClosing();
    }

}
