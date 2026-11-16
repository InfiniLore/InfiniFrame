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
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Invokes the specified callback on the native window thread.
        /// </summary>
        /// <param name="callback">The callback to execute.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow Invoke(Action callback) {
            _ = window.Features.Invoke.Invoke(callback);
            return window;
        }
        /// <summary>Dispatches work to the native window thread without blocking the caller.</summary>
        public ValueTask<InfiniFrameDispatchResult> DispatchAsync(
            Action callback,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default
        ) => window.Features.Invoke.DispatchAsync(callback, timeout, cancellationToken);
    }

}
