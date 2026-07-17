// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureLifecycle(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureLifecycle> logger,
    IValidator<InfiniFrameNativeParameters> validator
) : IInfiniFrameWindowFeatureLifecycle, IDisposable {
    private enum LifecycleStatus {
        Undefined = 0,
        Closing = 1,
        Closed = 2
    }
    
    private int _lifecycleState = (int)LifecycleStatus.Undefined;
    private int _messageLoopStarted;
    private int _messageLoopExited;
    private int _disposed;
    private int _nativeCallbackRootReleased;
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private LifecycleStatus LifecycleState {
        get => (LifecycleStatus)Volatile.Read(ref _lifecycleState);
        set => Volatile.Write(ref _lifecycleState, (int)value);
    }

    // Holds the native handle after MarkAsClosed zeros InstanceHandle but before Dispose frees it.
    private IntPtr _cleanupHandle = IntPtr.Zero;

    ~InfiniFrameWindowFeatureLifecycle() {
        Dispose(false);
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.CleanupNativeHandle"/>
    void IInfiniFrameWindowFeatureLifecycle.CleanupNativeHandle() {
        Dispose();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        // MarkAsClosed is invoked from the native closed callback, before WaitForExit has
        // returned. Deleting the native instance or unrooting reverse-P/Invoke delegates at
        // that point would race the remainder of WindowProc/WebView2 teardown. If a native
        // message loop is active, its finally block completes this deferred disposal.
        if (window.InstanceHandle != IntPtr.Zero ||
            (Volatile.Read(ref _messageLoopStarted) != 0 && Volatile.Read(ref _messageLoopExited) == 0))
            return;

        CleanupClosedHandleAndCallbacks(disposing);
    }

    private void CleanupClosedHandleAndCallbacks(bool disposing) {
        IntPtr handle = Interlocked.Exchange(ref _cleanupHandle, IntPtr.Zero);

        try {
            if (handle != IntPtr.Zero) InfiniFrameNative.Destructor(handle);
        }
        catch (Exception ex) when (!disposing && ExceptionsUtility.IsNonFatalException(ex)) {
            logger.LogTrace(ex, "Ignoring non-fatal exception while finalizing lifecycle cleanup.");
        }
        finally {
            ReleaseNativeCallbackRootOnce();
        }
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.Initialize"/>
    internal void Initialize() => window.Features.Lifecycle.Initialize();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.Initialize"/>
    void IInfiniFrameWindowFeatureLifecycle.Initialize() {
        InfiniFrameNativeParameters startupParameters = window.Configuration.StartupParameters;
        bool webInspectorEnabled = startupParameters.WebInspectorEnabled;

        try {
            if (startupParameters.RemoteDebuggingPort != 0) {
                logger.LogInformation(
                    "Remote debugging requested on loopback port {RemoteDebuggingPort}.",
                    startupParameters.RemoteDebuggingPort);

                if (OperatingSystem.IsLinux() && !startupParameters.DevToolsEnabled) {
                    logger.LogInformation(
                        "Linux remote debugging keeps WebKit developer extras enabled while active."
                    );
                }
            }
            else {
                logger.LogDebug("Remote debugging is disabled.");
            }

            RemoteDebuggingUtility.EnsureSupportedPlatform(startupParameters.RemoteDebuggingPort);
            RemoteDebuggingUtility.ValidatePortAvailabilityOrThrow(startupParameters.RemoteDebuggingPort, logger);
            if (webInspectorEnabled) {
                MacOsWebInspectorUtility.ThrowIfUnsupported();
            }

            validator.ValidateAndThrow(startupParameters);
            
            window.Events.OnWindowCreating();

            try {
                if (OperatingSystem.IsWindows()) {
                    InfiniFrameNative.RegisterWin32(window.MainProgramHandle);
                }
                else if (OperatingSystem.IsMacOS()) {
                    InfiniFrameNativeInteropStatus registerStatus = InfiniFrameNative.RegisterMac();
                    if (registerStatus != InfiniFrameNativeInteropStatus.Success) {
                        int lastError = Marshal.GetLastPInvokeError();
                        string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                        throw new ApplicationException(
                            $"Native registration failed with status {registerStatus}. Error #{lastError}. {nativeMessage}");
                    }
                }
                else if (OperatingSystem.IsLinux()) {} // No specific implementation for Linux
                else throw new PlatformNotSupportedException();

                InfiniFrameNativeInteropStatus status = InfiniFrameNative.Constructor(in startupParameters, out IntPtr handle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    
                    throw new ApplicationException(
                        $"Native constructor failed with status {status}. Error #{lastError}. {nativeMessage}");
                }
                
                ArgumentOutOfRangeException.ThrowIfZero(handle);
                window.InstanceHandle = handle;

                if (OperatingSystem.IsLinux()) {
                    NativeInvoke.InvokeSyncWithValidation(logger, handle, window.ManagedThreadId, () => {
                        window.SetManagedThreadId(Environment.CurrentManagedThreadId);
                    });
                }
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                int lastError = Marshal.GetLastPInvokeError();

                logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error #{lastError}", ex);
            }

            window.Events.OnWindowCreated();
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.WaitForClose"/>
    public void WaitForClose() {
        // Closing is not terminal. Close() may have queued the native close request before the
        // owning thread entered its message loop; that loop must still run to deliver the request.
        if (IsClosed()) {
            logger.LogDebug("Skipping WaitForClose because the window is already closed");
            return;
        }

        // A non-owning thread must not start a nested native message loop while the owning
        // thread is already pumping messages. It only needs to wait for the closed callback.
        if (!OperatingSystem.IsLinux() &&
            Environment.CurrentManagedThreadId != window.ManagedThreadId &&
            (LifecycleState is LifecycleStatus.Closing || Volatile.Read(ref _messageLoopStarted) != 0)) {
            _closed.Task.GetAwaiter().GetResult();
            return;
        }

        try {
            logger.LogDebug("Starting message loop for window.");
            if (OperatingSystem.IsLinux()) {
                // The GTK close request and its native closed callback can complete between
                // IsClosed() above and this call. Capture the owned native handle once; never
                // re-read InstanceHandle after the callback is allowed to clear it.
                IntPtr waitHandle = window.InstanceHandle;
                if (waitHandle == IntPtr.Zero) {
                    _closed.Task.GetAwaiter().GetResult();
                    return;
                }

                Volatile.Write(ref _messageLoopStarted, 1);
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.WaitForExit(waitHandle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int linuxLastError = Marshal.GetLastPInvokeError();
                    string linuxMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new ApplicationException(
                        $"Native WaitForExit failed with status {status}. Error #{linuxLastError}. {linuxMessage}");
                }
            }
            else {
                window.Features.Invoke.Invoke(() => {
                    if (IsClosed()) {
                        logger.LogDebug("Window closed whilst dispatching to the window thread. Skipping WaitForExit call.");
                        return;
                    }

                    Volatile.Write(ref _messageLoopStarted, 1);
                    InfiniFrameNative.WaitForExit(window.InstanceHandle);
                });
            }
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            int lastError = Marshal.GetLastPInvokeError();

            logger.LogError(ex, "Error #{LastErrorCode} while running message loop", lastError);
            throw new ApplicationException(
                $"Native code exception. Error #{lastError}",
                ex);
        }
        finally {
            MarkAsClosed();
            Volatile.Write(ref _messageLoopExited, 1);
            if (Volatile.Read(ref _disposed) != 0)
                CleanupClosedHandleAndCallbacks(true);
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.WaitForCloseAsync"/>
    public ValueTask WaitForCloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        if (IsClosed())
            return ValueTask.CompletedTask;

        WaitForClose();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.Close"/>
    public void Close() {
        if (Interlocked.CompareExchange(
                ref _lifecycleState,
                (int)LifecycleStatus.Closing,
                (int)LifecycleStatus.Undefined) != (int)LifecycleStatus.Undefined) {
            logger.LogDebug("Skipping Close during shutdown");
            return;
        }

        logger.LogDebug(".Close()");
        window.Events.OnWindowClosingRequested();

        IntPtr handle = window.InstanceHandle;
        if (handle == IntPtr.Zero) {
            logger.LogDebug("Skipping Close because window is not initialized");
            LifecycleState = LifecycleStatus.Closed;
            return;
        }

        NativeInvoke.InvokeSyncWithValidation(logger, handle, window.ManagedThreadId, InfiniFrameNative.Close);
        // The native close operation is asynchronous on Windows. OnWindowClosed/WaitForClose owns
        // the transition to Closed and handle release after the native window is actually gone.
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.CloseAsync"/>
    public ValueTask CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        Close();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="InfiniFrameWindowFeatureLifecycle.MarkAsClosed"/>
    internal void MarkAsClosed() => window.Features.Lifecycle.MarkAsClosed();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.MarkAsClosed"/>
    void IInfiniFrameWindowFeatureLifecycle.MarkAsClosed() {
        if (Interlocked.Exchange(ref _lifecycleState, (int)LifecycleStatus.Closed) == (int)LifecycleStatus.Closed) {
            _closed.TrySetResult();
            return;
        }

        IntPtr handle = window.InstanceHandle;
        window.InstanceHandle = IntPtr.Zero;

        if (handle != IntPtr.Zero) {
            // Destructor is intentionally NOT called here. MarkAsClosed runs inside the native
            // closed/destroy callback on every platform. The native object remains owned by this
            // lifecycle instance until WaitForExit has unwound and CleanupNativeHandle is called.
            _ = Interlocked.CompareExchange(ref _cleanupHandle, handle, IntPtr.Zero);
        }

        _closed.TrySetResult();
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureLifecycle.IsClosedOrClosing"/>
    public bool IsClosedOrClosing() {
        if (LifecycleState is LifecycleStatus.Closed or LifecycleStatus.Closing) return true;
        return window.InstanceHandle == IntPtr.Zero;
    }

    private bool IsClosed()
        => LifecycleState is LifecycleStatus.Closed || window.InstanceHandle == IntPtr.Zero;

    private void ReleaseNativeCallbackRootOnce() {
        if (Interlocked.Exchange(ref _nativeCallbackRootReleased, 1) != 0) return;

        try {
            window.Events.ReleaseNativeCallbackRoot();
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            logger.LogTrace(ex, "Ignoring non-fatal exception while releasing native callback root.");
        }
    }
}
