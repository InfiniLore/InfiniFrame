// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureInvokeExtensions {
    /// <summary>
    ///     Invokes the specified callback on the native window thread.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="callback">The callback to execute.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow Invoke(this IInfiniFrameWindow window, Action callback) {
        window.Features.Invoke.Invoke(callback);
        return window;
    }
}
