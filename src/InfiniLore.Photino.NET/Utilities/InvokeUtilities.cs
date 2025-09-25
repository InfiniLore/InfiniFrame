namespace InfiniLore.Photino.NET.Utilities;
internal static class InvokeUtilities {
    public static T? InvokeAndReturn<T>(IPhotinoWindow window, Func<IntPtr, T> callback) {
        T? value = default;
        window.Invoke(() => value = callback(window.InstanceHandle));
        return value;
    }

    public static T? InvokeAndReturn<T>(IPhotinoWindow window, FuncWithOut<T> callback) {
        T? value = default;
        window.Invoke(() => callback(window.InstanceHandle, out value));
        return value;
    }

    internal delegate void FuncWithOut<T>(IntPtr handle, out T value);
}
