// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LifecycleInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<LifecycleInfiniFrameWindowFeature> logger,
    IValidator<InfiniFrameNativeParameters> validator
) : ILifecycleInfiniFrameWindowFeature, IDisposable {
    public InfiniFrameWindowLifecycleState State => window.LifecycleState;
    private int _messageLoopStarted;
    private int _messageLoopExited;
    private int _closeRequestDispatched;
    private int _disposed;
    private int _nativeCallbackRootReleased;
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _messageLoopCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Provides the lifecycle management features for an InfiniFrame window.
    ///     Implements both <see cref="ILifecycleInfiniFrameWindowFeature" /> and <see cref="IDisposable" /> to handle
    ///     the state transitions and resource cleanup related to the lifecycle of the window.
    /// </summary>
    ~LifecycleInfiniFrameWindowFeature() {
        Dispose(false);
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.CleanupNativeHandle" />
    void ILifecycleInfiniFrameWindowFeature.CleanupNativeHandle() {
        Dispose();
    }

    bool ILifecycleInfiniFrameWindowFeature.CanWaitForCloseDuringDispose() {
        if (Volatile.Read(ref _closeRequestDispatched) == 0) return false;

        return Volatile.Read(ref _messageLoopStarted) == 0
            || Environment.CurrentManagedThreadId != window.ManagedThreadId;
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
        if (window.LifecycleState < InfiniFrameWindowLifecycleState.NativeClosed)
            return;

        if (Volatile.Read(ref _messageLoopStarted) != 0 && Volatile.Read(ref _messageLoopExited) == 0) {
            // Disposal from a native callback on the owning thread must be deferred until
            // WaitForExit unwinds. Other threads can deterministically wait for that boundary.
            if (Environment.CurrentManagedThreadId == window.ManagedThreadId) return;

            _messageLoopCompleted.Task.GetAwaiter().GetResult();
            return;
        }

        CleanupClosedHandleAndCallbacks(disposing);
    }

    private void CleanupClosedHandleAndCallbacks(bool disposing) {
        try {
            window.ReleaseNativeHandle();
            window.MarkDisposed();
        }
        catch (Exception ex) when (!disposing && ExceptionsUtility.IsNonFatalException(ex)) {
            logger.LogTrace(ex, "Ignoring non-fatal exception while finalizing lifecycle cleanup.");
        }
        finally {
            ReleaseNativeCallbackRootOnce();
        }
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.Initialize" />
    void ILifecycleInfiniFrameWindowFeature.Initialize() {
        window.BeginInitialization();
        InfiniFrameNativeParameters startupParameters = window.Configuration.StartupParameters;
        bool webInspectorEnabled = startupParameters.WebInspectorEnabled;

        try {
            if (startupParameters.RemoteDebuggingPort != 0) {
                logger.LogInformation(
                    "Remote debugging requested on loopback port {RemoteDebuggingPort}.",
                    startupParameters.RemoteDebuggingPort
                );

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
            if (webInspectorEnabled) MacOsWebInspectorUtility.ThrowIfUnsupported();

            validator.ValidateAndThrow(startupParameters);

            window.Events.OnWindowCreating();

            try {
                if (OperatingSystem.IsWindows()) InfiniFrameNative.RegisterWin32(window.MainProgramHandle);
                else if (OperatingSystem.IsMacOS()) {
                    InfiniFrameNativeInteropStatus registerStatus = InfiniFrameNative.RegisterMac();
                    if (registerStatus != InfiniFrameNativeInteropStatus.Success) {
                        int lastError = Marshal.GetLastPInvokeError();
                        string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                        throw new ApplicationException(
                            $"Native registration failed with status {registerStatus}. Error #{lastError}. {nativeMessage}");
                    }
                }
                else if (OperatingSystem.IsLinux()) {}// No specific implementation for Linux
                else throw new PlatformNotSupportedException();

                using NativeHandleLease? parentLease = window.Configuration.ParentWindow is {} parent
                    ? parent.AcquireNativeHandle()
                    : null;
                startupParameters.NativeParent = parentLease?.Handle ?? IntPtr.Zero;

                InfiniFrameNativeInteropStatus status = InfiniFrameNative.Constructor(in startupParameters, out IntPtr handle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";

                    throw new ApplicationException(
                        $"Native constructor failed with status {status}. Error #{lastError}. {nativeMessage}"
                    );
                }

                ArgumentOutOfRangeException.ThrowIfZero(handle);
                window.AssignNativeHandle(handle);

                if (OperatingSystem.IsLinux()) {
                    NativeInvoke.InvokeSyncWithValidation(logger, window, window.ManagedThreadId, callback: () => {
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
        catch {
            window.ReleaseNativeHandle();
            window.MarkDisposed();
            ReleaseNativeCallbackRootOnce();
            throw;
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.WaitForClose" />
    public void WaitForClose() {
        // Closing is not terminal. Close() may have queued the native close request before the
        // owning thread entered its message loop; that loop must still run to deliver the request.
        if (IsClosed()) {
            logger.LogDebug("Skipping WaitForClose because the window is already closed");
            return;
        }

        // AppKit is owned by the process main thread. A macOS caller on another thread must
        // only observe the closed callback: marshaling WaitForExit to the main thread would
        // start a nested NSRunLoop that does not reliably drain main-queue dispatch callbacks.
        // On Windows, a non-owning thread can observe an already-running (or closing) window,
        // but the owning thread must still start the native message loop when none exists.
        bool isNonOwningThread = Environment.CurrentManagedThreadId != window.ManagedThreadId;
        bool canObserveNativeClose = OperatingSystem.IsMacOS()
            || OperatingSystem.IsWindows()
            && (window.LifecycleState == InfiniFrameWindowLifecycleState.ClosingRequested
                || Volatile.Read(ref _messageLoopStarted) != 0);
        if (isNonOwningThread && canObserveNativeClose) {
            _closed.Task.GetAwaiter().GetResult();
            return;
        }

        try {
            logger.LogDebug("Starting message loop for window.");
            if (OperatingSystem.IsLinux()) {
                Volatile.Write(ref _messageLoopStarted, 1);
                NativeHandleLease lease;
                try {
                    lease = window.AcquireNativeHandle(NativeHandleAccess.WaitForExit);
                }
                catch (ObjectDisposedException) when (IsClosed()) {
                    _closed.Task.GetAwaiter().GetResult();
                    return;
                }

                using (lease) {
                    InfiniFrameNativeInteropStatus status = InfiniFrameNative.WaitForExit(lease.Handle);
                    if (status == InfiniFrameNativeInteropStatus.Success) return;

                    int linuxLastError = Marshal.GetLastPInvokeError();
                    string linuxMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new ApplicationException(
                        $"Native WaitForExit failed with status {status}. Error #{linuxLastError}. {linuxMessage}"
                    );
                }
            }
            else {
                NativeInvoke.InvokeSyncForLifecycle(logger, window, window.ManagedThreadId,
                    NativeHandleAccess.WaitForExit, callback: handle => {
                        Volatile.Write(ref _messageLoopStarted, 1);
                        return InfiniFrameNative.WaitForExit(handle);
                    }
                );
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
            try {
                if (Volatile.Read(ref _disposed) != 0) CleanupClosedHandleAndCallbacks(true);
            }
            finally {
                _messageLoopCompleted.TrySetResult();
            }
        }
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.WaitForCloseAsync" />
    public ValueTask WaitForCloseAsync(CancellationToken ct = default) =>
        // The native message loop is owned by WaitForClose.  Starting that loop from an
        // asynchronous API would either block the caller or require moving UI work to a
        // thread-pool thread.  The closed callback is the authoritative native completion
        // signal, so this API only observes that signal and never pumps or blocks a thread.
        new(_closed.Task.WaitAsync(ct));

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.Close" />
    public void Close() {
        if (!window.RequestClose()) {
            logger.LogDebug("Skipping Close during shutdown");
            return;
        }

        logger.LogDebug(".Close()");
        window.Events.OnWindowClosingRequested();

        try {
            NativeInvoke.InvokeSyncForLifecycle(logger, window, window.ManagedThreadId,
                NativeHandleAccess.Close, InfiniFrameNative.Close);
        }
        finally {
            Volatile.Write(ref _closeRequestDispatched, 1);
        }
        // The native close operation is asynchronous on Windows. OnWindowClosed/WaitForClose owns
        // the transition to Closed and handle release after the native window is actually gone.
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.CloseAsync" />
    public ValueTask CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        Close();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc cref="LifecycleInfiniFrameWindowFeature.MarkAsClosed" />
    private void MarkAsClosed() => window.Features.Lifecycle.MarkAsClosed();

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.MarkAsClosed" />
    void ILifecycleInfiniFrameWindowFeature.MarkAsClosed() {
        if (window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) {
            _closed.TrySetResult();
            return;
        }

        window.MarkNativeClosed();
        _closed.TrySetResult();
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.IsClosedOrClosing" />
    public bool IsClosedOrClosing() => window.LifecycleState >= InfiniFrameWindowLifecycleState.ClosingRequested;

    private bool IsClosed()
        => window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed;

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
