// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="ILifecycleInfiniFrameWindowFeature"/> on <see cref="IInfiniFrameWindow"/>.
/// </summary>
public static class ILifecycleInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Waits until the window is ready for interaction.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask WaitForReadyAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForReadyAsync(ct);

    /// <summary>
    ///     Blocks the calling thread until the window is closed.
    /// </summary>
    /// <param name="window">The window instance.</param>
    public static void WaitForClose(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.WaitForClose();

    /// <summary>
    ///     Asynchronously waits until the window is closed.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask WaitForCloseAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForCloseAsync(ct);

    /// <summary>
    ///     Closes the window synchronously.
    /// </summary>
    /// <param name="window">The window instance.</param>
    public static void Close(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.Close();

    /// <summary>
    ///     Closes the window asynchronously.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask CloseAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.CloseAsync(ct);

    /// <summary>
    ///     Waits for closed callbacks to complete.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask WaitForClosedCallbacksAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForClosedCallbacksAsync(ct);

    /// <summary>
    ///     Waits for the window teardown to complete.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask WaitForTeardownAsync(this IInfiniFrameWindow window, CancellationToken ct = default)
        => window.Features.Lifecycle.WaitForTeardownAsync(ct);

    /// <summary>
    ///     Returns whether the window is closed or in the closing process.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns><c>true</c> if the window is closed or closing; otherwise, <c>false</c>.</returns>
    public static bool IsClosedOrClosing(this IInfiniFrameWindow window)
        => window.Features.Lifecycle.IsClosedOrClosing();
}
