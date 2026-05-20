// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Utility methods for invoking native callbacks on the UI thread and returning their results.
///     <para>
///         <b>Contract:</b> All overloads rely on <see cref="IInfiniFrameWindow.Invoke" /> being <b>synchronous</b>.
///         The callback must have completed by the time <c>Invoke</c> returns. If <c>Invoke</c> is ever made
///         asynchronous, these methods will return stale/default values and must be rewritten to use
///         a synchronization primitive (e.g. <see cref="System.Threading.ManualResetEventSlim" />).
///     </para>
/// </summary>
internal static class InvokeUtility {
    public static T? InvokeAndReturn<T>(IInfiniFrameWindow window, Func<IInfiniFrameWindow, T> callback) {
        T? value = default;
        // ReSharper disable once RedundantAssignment
        bool completed = false;
        window.Invoke(() => {
            value = callback(window);
            completed = true;
        });
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        return value;
    }

    public static T? InvokeAndReturn<T>(IInfiniFrameWindow window, Func<IntPtr, T> callback) {
        T? value = default;
        // ReSharper disable once RedundantAssignment
        bool completed = false;
        window.Invoke(() => {
            value = callback(window.InstanceHandle);
            completed = true;
        });
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        return value;
    }

    public static T InvokeAndReturn<T>(IInfiniFrameWindow window, FuncWithOut<T> callback) {
        T? value = default;
        // ReSharper disable once RedundantAssignment
        bool completed = false;
        window.Invoke(() => {
            callback(window.InstanceHandle, out value);
            completed = true;
        });
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        return value!;
    }

    public static T InvokeAndReturn<T, TResult>(IInfiniFrameWindow window, FuncWithOutResult<T, TResult> callback, Action<TResult>? validateResult = null) {
        T? value = default;
        TResult? result = default;
        // ReSharper disable once RedundantAssignment
        bool completed = false;
        window.Invoke(() => {
            result = callback(window.InstanceHandle, out value);
            completed = true;
        });
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        if (validateResult is not null && result is not null) {
            validateResult(result);
        }
        return value!;
    }

    internal delegate void FuncWithOut<T>(IntPtr handle, out T value);
    internal delegate TResult FuncWithOutResult<T, TResult>(IntPtr handle, out T value);
}
