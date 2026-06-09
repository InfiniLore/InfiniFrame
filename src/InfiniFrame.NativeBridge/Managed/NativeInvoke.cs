// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeInvoke {
    private const string NoNativeMessage = "No native error message provided.";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------    
    #region WithValidation
    internal static void InvokeSyncWithValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Action callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            });

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback);

        EnsureSuccess(logger, status);
    }

    internal static T? InvokeSyncWithValidation<T>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(logger, result.Status);
        return result.Value;
    }

    internal static void InvokeSyncWithValidation<T>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2> callback,
        T1 arg1,
        T2 arg2
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3, T4> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4)
        );

        EnsureSuccess(logger, status);
    }
    
    internal static void InvokeSyncWithValidation<T1, T2, T3, T4>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        ShowOpenDialogFoldersFunc<T1, T2, T3, T4> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        out T4? arg4
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        arg4 = default;
        T4? arg4Temp = default;
        
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, out arg4Temp)
        );
        
        arg4 = arg4Temp;
        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3, T4, T5> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, arg5)
        );

        EnsureSuccess(logger, status);
    }
    
    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        ShowMessageFunc<T1, T2, T3, T4, T5> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        out T5? arg5
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        arg5 = default;
        T5? arg5Temp = default;
        
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, out arg5Temp)
        );

        arg5 = arg5Temp;
        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5, T6>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3, T4, T5, T6> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, arg5, arg6)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5, T6>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        ShowSaveFileFunc<T1, T2, T3, T4, T5, T6> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        out T6? arg6
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        arg6 = default;
        T6? arg6Temp = default;
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, arg5, out arg6Temp));
        
        arg6 = arg6Temp;
        
        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5, T6, T7>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3, T4, T5, T6, T7> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithValidation<T1, T2, T3, T4, T5, T6, T7, T8>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T1, T2, T3, T4, T5, T6, T7, T8> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(
                windowInstanceHandle,
                arg1,
                arg2,
                arg3,
                arg4,
                arg5,
                arg6,
                arg7,
                arg8)
        );

        EnsureSuccess(logger, status);
    }
    #endregion
    #region WithoutValidation
    internal static void InvokeSyncWithoutValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Action callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                callback();
                return InfiniFrameNativeInteropStatus.Success;
            }
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithoutValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<IntPtr, InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle)
        );

        EnsureSuccess(logger, status);
    }

    internal static void InvokeSyncWithoutValidation(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        Func<InfiniFrameNativeInteropStatus> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback
        );

        EnsureSuccess(logger, status);
    }

    internal static T? InvokeSyncWithoutValidation<T>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithOut<T> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        (InfiniFrameNativeInteropStatus Status, T? Value) result = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => {
                InfiniFrameNativeInteropStatus status = callback(windowInstanceHandle, out T value);
                return (status, value);
            });

        EnsureSuccess(logger, result.Status);
        return result.Value;
    }

    internal static void InvokeSyncWithoutValidation<T>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        FuncWithArgs<T> callback,
        T arg
    ) {
        ArgumentNullException.ThrowIfNull(callback);

        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg)
        );

        EnsureSuccess(logger, status);
    }
    #endregion

    private static TResult? ExecuteInvokeSync<TResult>(
        ILogger logger,
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
                logger.LogDebug("Executing callback on same thread");
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
            logger.LogDebug("Executing callback on window thread. Marshalling to C++ native cobebase.");
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

    private static void EnsureSuccess(ILogger logger, InfiniFrameNativeInteropStatus status) {
        int fallbackLastError = Marshal.GetLastPInvokeError();

        if (status is InfiniFrameNativeInteropStatus.Success && fallbackLastError is 0) {
            logger.LogDebug("Native interop call succeeded with no error.");
            return;
        }

        logger.LogCritical("Native interop call failed with unknown status state. Fallback last error {FallbackLastError} whilst the received status is {FallbackStatus}", fallbackLastError, status);

        string message;
        string? foundMessage = InfiniFrameNative.GetLastErrorMessage();
        if (foundMessage is not null) {
            logger.LogDebug("Native interop call failed with error: {FoundMessage}", foundMessage);
            message = foundMessage;
        }
        else {
            logger.LogDebug("Native interop call failed with no error message.");
            message = NoNativeMessage;
        }


        InfiniFrameNativeInteropStatus actualStatus = status;
        if (foundMessage is not null) {
            actualStatus = InfiniFrameNativeInteropStatus.OperationFailed;
            logger.LogDebug("Overwriting original status of {InfiniFrameNativeInteropStatus} with {ActualStatus}", status, actualStatus);
        }

        logger.LogCritical("Native interop call failed with unknown status state. Fallback last error {FallbackLastError}. {FallbackMessage} {FallbackStatus}", fallbackLastError, message, actualStatus);
        throw new ApplicationException($"Native interop call failed with unknown status state. Fallback last error {fallbackLastError}. {message} {actualStatus}");
    }

    internal delegate InfiniFrameNativeInteropStatus FuncWithOut<T>(IntPtr handle, out T value);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T>(IntPtr handle, T arg);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2>(IntPtr handle, T1 arg, T2 arg2);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3>(IntPtr handle, T1 arg, T2 arg2, T3 arg3);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4);
    
    internal delegate InfiniFrameNativeInteropStatus ShowOpenDialogFoldersFunc<in T1, in T2, in T3, T4>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, out T4? arg4);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    
    internal delegate InfiniFrameNativeInteropStatus ShowMessageFunc<in T1, in T2, in T3, in T4, T5>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, out T5 arg5);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    internal delegate InfiniFrameNativeInteropStatus ShowSaveFileFunc<in T1, in T2, in T3, in T4, in T5, T6>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out T6? arg6);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
}
