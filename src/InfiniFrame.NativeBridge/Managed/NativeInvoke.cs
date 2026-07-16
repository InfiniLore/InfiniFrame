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
    internal static void InvokeSyncWithValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Action callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            });

        EnsureSuccess(status);
    }

    internal static void InvokeSyncWithValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(status);
    }

    internal static void InvokeSyncWithValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback);

        EnsureSuccess(status);
    }

    internal static T? InvokeSyncWithValidation<T>(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(result.Status);
        return result.Value;
    }

    internal static void InvokeSyncWithValidation<T>(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(status);
    }
    #endregion
    #region WithoutValidation
    internal static void InvokeSyncWithoutValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Action callback) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            }
        );

        EnsureSuccess(status);
    }

    internal static void InvokeSyncWithoutValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(status);
    }

    internal static void InvokeSyncWithoutValidation(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback
        );

        EnsureSuccess(status);
    }

    internal static T? InvokeSyncWithoutValidation<T>(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(result.Status);
        return result.Value;
    }

    internal static void InvokeSyncWithoutValidation<T>(
        IntPtr windowInstanceHandle,
        int managedThreadId, 
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(status);
    }
    #endregion

    private static TResult? ExecuteInvokeSync<TResult>(
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<TResult> callback
    ) {
        TResult? result = default;
        Exception? callbackException = null;
        bool completed = false;

        Marshal.SetLastPInvokeError(0);

        // If the callback is being executed on the same thread, we can execute it synchronously.
        if (Environment.CurrentManagedThreadId == managedThreadId) {
            try {
                result = callback();
            }
            catch (Exception ex) {
                callbackException = ex;
            }
            finally {
                completed = true;
            }
        }
        
        // Otherwise, we need to execute it on the window thread.
        else {
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
        }

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

    internal delegate InfiniFrameNativeInteropStatus FuncWithOut<T>(IntPtr handle, out T value);
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T>(IntPtr handle, T arg);
}
