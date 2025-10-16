// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InvokeUtilities {
    public static T? InvokeAndReturn<T>(IInfiniWindow window, Func<IInfiniWindow, T> callback) {
        T? value = default;
        window.Invoke(() => value = callback(window));
        return value;
    }
    public static T? InvokeAndReturn<T>(IInfiniWindow window, Func<IntPtr, T> callback) {
        T? value = default;
        window.Invoke(() => value = callback(window.InstanceHandle));
        return value;
    }

    public static T? InvokeAndReturn<T>(IInfiniWindow window, FuncWithOut<T> callback) {
        T? value = default;
        window.Invoke(() => callback(window.InstanceHandle, out value));
        return value;
    }

    internal delegate void FuncWithOut<T>(IntPtr handle, out T value);
}
