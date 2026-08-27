// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IInvokeInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IInvokeInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Invokes the specified callback on the native window thread.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="callback">The callback to execute.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow Invoke(this IInfiniFrameWindow window, Action callback) {
        _ = window.Features.Invoke.Invoke(callback);
        return window;
    }

    /// <summary>Dispatches work to the native window thread without blocking the caller.</summary>
    public static ValueTask<InfiniFrameDispatchResult> DispatchAsync(
        this IInfiniFrameWindow window,
        Action callback,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) => window.Features.Invoke.DispatchAsync(callback, timeout, cancellationToken);
}
