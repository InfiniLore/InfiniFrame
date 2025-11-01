// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InvokeUtilities {
    public static T? InvokeAndReturn<T>(IInfiniFrameWindow window, Func<IInfiniFrameWindow, T> callback) {
        T? value = default;
        window.Invoke(() => value = callback(window));
        return value;
    }
    public static T? InvokeAndReturn<T>(IInfiniFrameWindow window, Func<IntPtr, T> callback) {
        T? value = default;
        window.Invoke(() => value = callback(window.InstanceHandle));
        return value;
    }

    public static T InvokeAndReturn<T>(IInfiniFrameWindow window, FuncWithOut<T> callback) {
        T? value = default;
        window.Invoke(() => callback(window.InstanceHandle, out value));
        return value!;
    }

    internal delegate void FuncWithOut<T>(IntPtr handle, out T value);
}
