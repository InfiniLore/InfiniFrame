// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeInvoke {
    #region WithValidation
    internal static void InvokeWithValidation(IntPtr windowInstanceHandle, Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            });

        EnsureSuccess(status);
    }

    internal static void InvokeWithValidation(
        IntPtr windowInstanceHandle,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(status);
    }

    internal static void InvokeWithValidation(
        IntPtr windowInstanceHandle,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback);

        EnsureSuccess(status);
    }

    internal static T? InvokeWithValidation<T>(
        IntPtr windowInstanceHandle,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(result.Status);
        return result.Value;
    }

    internal static void InvokeWithValidation<T>(
        IntPtr windowInstanceHandle,
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(status);
    }
    #endregion
    
    #region WithoutValidation
    internal static void InvokeWithoutValidation(IntPtr windowInstanceHandle, Action callback) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            }
        );

        EnsureSuccess(status);
    }

    internal static void InvokeWithoutValidation(
        IntPtr windowInstanceHandle,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(status);
    }

    internal static void InvokeWithoutValidation(
        IntPtr windowInstanceHandle,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback
        );

        EnsureSuccess(status);
    }

    internal static T? InvokeWithoutValidation<T>(
        IntPtr windowInstanceHandle,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(result.Status);
        return result.Value;
    }

    internal static void InvokeWithoutValidation<T>(
        IntPtr windowInstanceHandle,
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvoke(
            windowInstanceHandle,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(status);
    }
    #endregion

    private static TResult? ExecuteInvoke<TResult>(
        IntPtr windowInstanceHandle,
        Func<TResult> callback
    ) {
        TResult? result = default;
        Exception? callbackException = null;
        bool completed = false;

        Marshal.SetLastPInvokeError(0);
        
        InfiniFrameNative.Invoke(windowInstanceHandle, callback: () => {
            try {
                result = callback();
            }
            catch (Exception ex) {
                callbackException = ex;
            }
            finally {
                completed = true;
            }
        });

        if (!completed) throw new InvalidOperationException("InfiniFrameNative.Invoke must execute synchronously. The callback did not complete before Invoke returned.");
        if (callbackException is not null) ExceptionDispatchInfo.Capture(callbackException).Throw();

        return result;
    }
    
    private static void EnsureSuccess(InfiniFrameNativeInteropStatus status) {
        int fallbackLastError = Marshal.GetLastPInvokeError();

        if (status is InfiniFrameNativeInteropStatus.Success && fallbackLastError is 0) return;

        const string noNativeMessage = "No native error message provided.";
        string fallbackMessage = InfiniFrameNative.GetLastErrorMessage() ?? noNativeMessage;
        InfiniFrameNativeInteropStatus fallbackStatus = fallbackMessage == noNativeMessage
            ? InfiniFrameNativeInteropStatus.OperationFailed
            : InfiniFrameNativeInteropStatus.Success;

        throw new ApplicationException($"Native interop call failed with unknown status state. Fallback last error {fallbackLastError}. {fallbackMessage} {fallbackStatus}");
    }

    internal delegate InfiniFrameNativeInteropStatus FuncWithOut<T>( IntPtr handle, out T value);
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T>(IntPtr handle, T arg);
}
