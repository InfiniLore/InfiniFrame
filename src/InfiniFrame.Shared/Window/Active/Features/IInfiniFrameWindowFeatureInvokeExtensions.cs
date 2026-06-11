// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureInvokeExtensions {
    public static IInfiniFrameWindow Invoke(this IInfiniFrameWindow window, Action callback) {
        window.Features.Invoke.Invoke(callback);
        return window;
    }
}
