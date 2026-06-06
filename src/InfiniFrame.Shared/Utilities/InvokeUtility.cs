// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
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
    public static void NativeInvokeWithValidation(IntPtr windowInstanceHandle, Func<IntPtr, InfiniFrameNativeInteropStatus> callback) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowInstanceHandle);
        ArgumentNullException.ThrowIfNull(callback);
        
        InfiniFrameNativeInteropStatus status = default;
        bool completed = false;
        
        InfiniFrameNative.Invoke(windowInstanceHandle, () => {
            status = callback(windowInstanceHandle);
            completed = true;
        });
        
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? reason)) {
            throw new ApplicationException($"Native interop call failed with status {status}. {reason}");
        }
    }
    
    public static void NativeInvokeWithValidation(IntPtr windowInstanceHandle, Func<InfiniFrameNativeInteropStatus> callback) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowInstanceHandle);
        ArgumentNullException.ThrowIfNull(callback);
        
        InfiniFrameNativeInteropStatus status = default;
        bool completed = false;
        
        InfiniFrameNative.Invoke(windowInstanceHandle, () => {
            status = callback();
            completed = true;
        });
        
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? reason)) {
            throw new ApplicationException($"Native interop call failed with status {status}. {reason}");
        }
    }
    
    public static T? NativeInvokeWithValidation<T>(IntPtr windowInstanceHandle, FuncWithOut<T> callback) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowInstanceHandle);
        ArgumentNullException.ThrowIfNull(callback);

        T? value = default;
        InfiniFrameNativeInteropStatus status = default;
        bool completed = false;
        
        InfiniFrameNative.Invoke(windowInstanceHandle, () => {
            status = callback(windowInstanceHandle, out value);
            completed = true;
        });
        
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? reason)) {
            throw new ApplicationException($"Native interop call failed with status {status}. {reason}");
        }
        
        return value;
    }
    
    public static void NativeInvokeWithValidation<T>(IntPtr windowInstanceHandle, FuncWithArgs<T> callback, T arg) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowInstanceHandle);
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = default;
        bool completed = false;
        
        InfiniFrameNative.Invoke(windowInstanceHandle, () => {
            status = callback(windowInstanceHandle, arg);
            completed = true;
        });
        
        Debug.Assert(completed, "Invoke must be synchronous, callback did not complete before Invoke returned.");
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? reason)) {
            throw new ApplicationException($"Native interop call failed with status {status}. {reason}");
        }
    }

    internal delegate InfiniFrameNativeInteropStatus FuncWithOut<T>(IntPtr handle, out T value);
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T>(IntPtr handle, T arg);
}
