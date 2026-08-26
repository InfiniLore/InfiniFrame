// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInvokeInfiniFrameWindowFeature {
    /// <summary>
    ///     Invokes the specified callback on the native window thread.
    /// </summary>
    /// <param name="callback">The callback to execute.</param>
    InfiniFrameDispatchResult Invoke(Action callback);

    /// <summary>
    ///     Dispatches work without blocking the caller. A callback that has not started when cancellation, timeout, or
    ///     window shutdown wins is suppressed.
    /// </summary>
    ValueTask<InfiniFrameDispatchResult> DispatchAsync(
        Action callback,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    );
}
