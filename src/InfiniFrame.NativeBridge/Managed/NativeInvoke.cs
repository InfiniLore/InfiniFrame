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
/// <summary>
///     Provides helper methods for invoking native interop calls synchronously on the correct thread,
///     with optional argument validation and error handling.
/// </summary>
internal static class NativeInvoke {
    private const string NoNativeMessage = "No native error message provided.";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------    
    #region WithValidation
    /// <summary>
    ///     Invokes a synchronous callback on the window thread with validation of arguments and native status.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The action to execute.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback on the window thread with validation and a window handle parameter.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute, receiving the window handle.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback on the window thread with validation.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback on the window thread that produces an output value, with validation.
    /// </summary>
    /// <typeparam name="T">The type of the output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute, producing an output value.</param>
    /// <returns>The output value from the callback.</returns>
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

    /// <summary>
    ///     Invokes a synchronous callback with a single argument on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg">The argument to pass.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback with two arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback that returns two output values on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first output value.</typeparam>
    /// <typeparam name="T2">The type of the second output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <returns>A tuple containing the two output values.</returns>
    internal static (T1?, T2?) InvokeSyncWithValidation<T1, T2>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        GetSizeFunc<T1, T2> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        T1? arg1 = default;
        T2? arg2 = default;
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, out arg1, out arg2)
        );

        EnsureSuccess(logger, status);

        return (arg1, arg2);
    }

    /// <summary>
    ///     Invokes a synchronous callback with three arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback with four arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
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
    
    /// <summary>
    ///     Invokes a synchronous callback for opening a folder dialog, returning the selected path, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <returns>The output value from the callback.</returns>
    internal static T4? InvokeSyncWithValidation<T1, T2, T3, T4>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        ShowOpenDialogFoldersFunc<T1, T2, T3, T4> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);

        T4? arg4 = default;
        
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, out arg4)
        );
        
        EnsureSuccess(logger, status);
        return arg4;
    }
    
    /// <summary>
    ///     Invokes a synchronous callback that returns four output values (e.g. window rectangle), with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first output value.</typeparam>
    /// <typeparam name="T2">The type of the second output value.</typeparam>
    /// <typeparam name="T3">The type of the third output value.</typeparam>
    /// <typeparam name="T4">The type of the fourth output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <returns>A tuple containing the four output values.</returns>
    internal static (T1?, T2?, T3?, T4?) InvokeSyncWithValidation<T1, T2, T3, T4>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        GetWindowRectangleFunc<T1, T2, T3, T4> callback
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        T1? arg1 = default;
        T2? arg2 = default;
        T3? arg3 = default;
        T4? arg4 = default;
        
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, out arg1, out arg2, out arg3, out arg4)
        );

        EnsureSuccess(logger, status);
        return (arg1, arg2, arg3, arg4);
    }

    /// <summary>
    ///     Invokes a synchronous callback with five arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
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
    
    /// <summary>
    ///     Invokes a synchronous callback for showing a message dialog, producing an output value, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The output value.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback with six arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback for showing a save-file dialog, returning the selected path, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <returns>The output value from the callback.</returns>
    internal static T6? InvokeSyncWithValidation<T1, T2, T3, T4, T5, T6>(
        ILogger logger,
        IntPtr windowInstanceHandle,
        int managedThreadId,
        ShowSaveFileFunc<T1, T2, T3, T4, T5, T6> callback,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfZero(windowInstanceHandle);
        
        T6? arg6 = default;
        InfiniFrameNativeInteropStatus status = ExecuteInvokeSync(
            logger,
            windowInstanceHandle,
            managedThreadId,
            callback: () => callback(windowInstanceHandle, arg1, arg2, arg3, arg4, arg5, out arg6));
        
        EnsureSuccess(logger, status);
        return arg6;
    }

    /// <summary>
    ///     Invokes a synchronous callback with seven arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <param name="arg7">The seventh argument.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback with eight arguments on the window thread, with validation.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg1">The first argument.</param>
    /// <param name="arg2">The second argument.</param>
    /// <param name="arg3">The third argument.</param>
    /// <param name="arg4">The fourth argument.</param>
    /// <param name="arg5">The fifth argument.</param>
    /// <param name="arg6">The sixth argument.</param>
    /// <param name="arg7">The seventh argument.</param>
    /// <param name="arg8">The eighth argument.</param>
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
    /// <summary>
    ///     Invokes a synchronous callback on the window thread without argument validation, but with error checking.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The action to execute.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback on the window thread without argument validation, with error checking.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute, receiving the window handle.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback on the window thread without argument validation, with error checking.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
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

    /// <summary>
    ///     Invokes a synchronous callback that produces an output value, without argument validation.
    /// </summary>
    /// <typeparam name="T">The type of the output value.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <returns>The output value from the callback.</returns>
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

    /// <summary>
    ///     Invokes a synchronous callback with a single argument, without argument validation.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <param name="arg">The argument to pass.</param>
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

    /// <summary>
    ///     Executes a synchronous native invoke, marshalling to the window thread if necessary.
    /// </summary>
    /// <typeparam name="TResult">The return type of the callback.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="windowInstanceHandle">The native window handle.</param>
    /// <param name="managedThreadId">The managed thread ID of the window thread.</param>
    /// <param name="callback">The function to execute.</param>
    /// <returns>The result of the callback.</returns>
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

    /// <summary>
    ///     Ensures the native interop call succeeded; throws <see cref="ApplicationException"/> if it failed.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="status">The status returned from the native call.</param>
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

    /// <summary>
    ///     Represents a native interop callback that produces an output value.
    /// </summary>
    /// <typeparam name="T">The type of the output value.</typeparam>
    /// <param name="handle">The native window handle.</param>
    /// <param name="value">The output value.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal delegate InfiniFrameNativeInteropStatus FuncWithOut<T>(IntPtr handle, out T value);

    /// <summary>
    ///     Represents a native interop callback with a single argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="handle">The native window handle.</param>
    /// <param name="arg">The argument.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T>(IntPtr handle, T arg);

    /// <summary>
    ///     Represents a native interop callback with two arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2>(IntPtr handle, T1 arg, T2 arg2);
    
    /// <summary>
    ///     Represents a native interop callback that returns two output values.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus GetSizeFunc<T1, T2>(IntPtr handle, out T1 arg, out T2 arg2);

    /// <summary>
    ///     Represents a native interop callback with three arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3>(IntPtr handle, T1 arg, T2 arg2, T3 arg3);

    /// <summary>
    ///     Represents a native interop callback with four arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4);
    
    /// <summary>
    ///     Represents a native interop callback for opening a folder dialog.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus ShowOpenDialogFoldersFunc<in T1, in T2, in T3, T4>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, out T4? arg4);
    
    /// <summary>
    ///     Represents a native interop callback that returns four output values (e.g. window rectangle).
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus GetWindowRectangleFunc<T1, T2, T3, T4>(IntPtr handle, out T1? arg, out T2? arg2, out T3? arg3, out T4? arg4);

    /// <summary>
    ///     Represents a native interop callback with five arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    
    /// <summary>
    ///     Represents a native interop callback for showing a message dialog.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus ShowMessageFunc<in T1, in T2, in T3, in T4, T5>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, out T5 arg5);

    /// <summary>
    ///     Represents a native interop callback with six arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    /// <summary>
    ///     Represents a native interop callback for showing a save-file dialog.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus ShowSaveFileFunc<in T1, in T2, in T3, in T4, in T5, T6>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out T6? arg6);

    /// <summary>
    ///     Represents a native interop callback with seven arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    /// <summary>
    ///     Represents a native interop callback with eight arguments.
    /// </summary>
    internal delegate InfiniFrameNativeInteropStatus FuncWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(IntPtr handle, T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
}
