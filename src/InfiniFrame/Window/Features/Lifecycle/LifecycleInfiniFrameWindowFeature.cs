// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FluentValidation;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LifecycleInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<LifecycleInfiniFrameWindowFeature> logger,
    IValidator<InfiniFrameNativeParameters> validator
) : ILifecycleInfiniFrameWindowFeature, IDisposable {
    private static readonly InfiniFrameNative.ContextAction ReadyCallback = OnNativeReady;
    private static readonly InfiniFrameNative.ContextAction TeardownCallback = OnNativeTeardown;
    private readonly object _closeAttemptLock = new();
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _closedCallbacksDelivered = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _messageLoopCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _teardown = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _cleanupCompleted;
    private TaskCompletionSource? _closeAttempt;
    private int _closeRequestDispatched;
    private int _disposed;
    private int _messageLoopExited;
    private int _messageLoopStarted;
    private GCHandle _milestoneRoot;
    private int _milestoneRootReleased;
    private int _nativeCallbackRootReleased;

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    public InfiniFrameWindowLifecycleState State => window.LifecycleState;

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.CleanupNativeHandle" />
    void ILifecycleInfiniFrameWindowFeature.CleanupNativeHandle() {
        Dispose();
    }

    bool ILifecycleInfiniFrameWindowFeature.CanWaitForCloseDuringDispose() {
        if (Volatile.Read(ref _closeRequestDispatched) == 0) return false;

        return Volatile.Read(ref _messageLoopStarted) == 0
            || Environment.CurrentManagedThreadId != window.ManagedThreadId;
    }

    bool ILifecycleInfiniFrameWindowFeature.CanWaitForTeardownDuringDispose()
        // Never wait re-entrantly from a native callback on the owning loop: the
        // callback must return before WM_NCDESTROY can schedule teardown. Once
        // WaitForClose has exited, or from any non-owning thread, blocking is safe.
        => Volatile.Read(ref _messageLoopExited) != 0
            || Environment.CurrentManagedThreadId != window.ManagedThreadId;

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
                        throw new InfiniFrameNativeInteropException(
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

                    throw new InfiniFrameNativeInteropException(
                        $"Native constructor failed with status {status}. Error #{lastError}. {nativeMessage}"
                    );
                }

                ArgumentOutOfRangeException.ThrowIfZero(handle);
                window.AssignNativeHandle(handle);
                RegisterNativeMilestoneCallbacks(handle);

                if (OperatingSystem.IsLinux()) {
                    NativeInvoke.InvokeSyncWithValidation(logger, window, window.ManagedThreadId, callback: () => {
                        window.SetManagedThreadId(Environment.CurrentManagedThreadId);
                    });
                }
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                int lastError = Marshal.GetLastPInvokeError();

                logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new InfiniFrameNativeInteropException($"Native code exception. Error #{lastError}", ex);
            }

            window.Events.OnWindowCreated();
        }
        catch {
            window.ReleaseNativeHandle();
            window.MarkDisposed();
            ReleaseNativeCallbackRootOnce();
            ReleaseMilestoneRootOnce();
            throw;
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    public ValueTask WaitForReadyAsync(CancellationToken ct = default)
        => new(_ready.Task.WaitAsync(ct));

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
            // Blocking here is safe: we are on a non-owning thread, so we are not
            // holding a native message loop that would deadlock. The native loop
            // runs on the owning thread; we merely wait for its completion signal.
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
                    // Safe to block: the window was already closed, so the _closed signal
                    // has been set. We are on a thread that does not own the message loop.
                    _closed.Task.GetAwaiter().GetResult();
                    return;
                }

                using (lease) {
                    InfiniFrameNativeInteropStatus status = InfiniFrameNative.WaitForExit(lease.Handle);
                    if (status == InfiniFrameNativeInteropStatus.Success) return;

                    int linuxLastError = Marshal.GetLastPInvokeError();
                    string linuxMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
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
            throw new InfiniFrameNativeInteropException(
                $"Native code exception. Error #{lastError}",
                ex);
        }
        finally {
            MarkAsClosed();
            Volatile.Write(ref _messageLoopExited, 1);
            _messageLoopCompleted.TrySetResult();
        }
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.WaitForCloseAsync" />
    public ValueTask WaitForCloseAsync(CancellationToken ct = default) =>
        // The native message loop is owned by WaitForClose.  Starting that loop from an
        // asynchronous API would either block the caller or require moving UI work to a
        // thread-pool thread.  The closed callback is the authoritative native completion
        // signal, so this API only observes that signal and never pumps or blocks a thread.
        new(_closed.Task.WaitAsync(ct));

    public ValueTask WaitForClosedCallbacksAsync(CancellationToken ct = default)
        => new(_closedCallbacksDelivered.Task.WaitAsync(ct));

    public ValueTask WaitForTeardownAsync(CancellationToken ct = default)
        => new(_teardown.Task.WaitAsync(ct));

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.Close" />
    public void Close() {
        bool requested;
        lock (_closeAttemptLock) {
            requested = window.RequestClose();
            if (requested)
                _closeAttempt = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        if (!requested) {
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
    public async ValueTask CloseAsync(CancellationToken ct = default) {
        ct.ThrowIfCancellationRequested();
        Close();

        // A caller-owned Windows STA may build a window without ever entering
        // WaitForClose (for example, a short-lived BlazorWebView app). The posted
        // WM_CLOSE and any in-flight WebView2 initialization callbacks cannot run
        // unless that owning thread pumps messages. Establish the native loop here
        // only when no loop exists; never nest or block an already-running UI loop.
        if (OperatingSystem.IsWindows()
            && Environment.CurrentManagedThreadId == window.ManagedThreadId
            && Volatile.Read(ref _messageLoopStarted) == 0
            && !IsClosed()) {
            WaitForClose();
        }

        Task attempt;
        lock (_closeAttemptLock) {
            attempt = _closeAttempt?.Task ?? _closed.Task;
        }

        await attempt.WaitAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.MarkAsClosed" />
    void ILifecycleInfiniFrameWindowFeature.MarkAsClosed() {
        if (window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) {
            _closed.TrySetResult();
            return;
        }

        window.MarkNativeClosed();
        window.MarkTeardownPending();
        _ready.TrySetException(new ObjectDisposedException(nameof(InfiniFrameWindow), "The window closed before becoming ready."));
        _closed.TrySetResult();
        lock (_closeAttemptLock) {
            _closeAttempt?.TrySetResult();
        }
    }

    void ILifecycleInfiniFrameWindowFeature.MarkClosedCallbacksDelivered()
        => _closedCallbacksDelivered.TrySetResult();

    void ILifecycleInfiniFrameWindowFeature.MarkCloseRejected() {
        TaskCompletionSource? attempt;
        lock (_closeAttemptLock) {
            attempt = _closeAttempt;
        }

        attempt?.TrySetException(new InfiniFrameCloseRejectedException());
        Volatile.Write(ref _closeRequestDispatched, 0);
    }

    /// <inheritdoc cref="ILifecycleInfiniFrameWindowFeature.IsClosedOrClosing" />
    public bool IsClosedOrClosing() => window.LifecycleState >= InfiniFrameWindowLifecycleState.ClosingRequested;

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

    private void Dispose(bool disposing) {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        if (window.LifecycleState < InfiniFrameWindowLifecycleState.TeardownComplete
            && window.LifecycleState != InfiniFrameWindowLifecycleState.Disposed) {
            // The normal teardown path hasn't completed yet. Still release callback roots
            // and milestones to avoid leaks, and release the native handle so .NET 10's
            // runtime doesn't abort during shutdown over unreleased SafeHandles.
            ReleaseNativeCallbackRootOnce();
            ReleaseMilestoneRootOnce();
            try { window.ReleaseNativeHandle(); }
            catch {
                // ignored
            }

            try { window.MarkNativeHandleReleased(); }
            catch {
                // ignored
            }

            try { window.MarkDisposed(); }
            catch {
                // ignored
            }

            return;
        }

        CleanupClosedHandleAndCallbacks(disposing);
    }

    private void CleanupClosedHandleAndCallbacks(bool disposing) {
        if (Interlocked.Exchange(ref _cleanupCompleted, 1) != 0) return;

        try {
            window.ReleaseNativeHandle();
            window.MarkNativeHandleReleased();
            window.MarkDisposed();
        }
        catch (Exception ex) when (!disposing && ExceptionsUtility.IsNonFatalException(ex)) {
            logger.LogTrace(ex, "Ignoring non-fatal exception while finalizing lifecycle cleanup.");
        }
        finally {
            ReleaseNativeCallbackRootOnce();
            ReleaseMilestoneRootOnce();
        }
    }

    /// <inheritdoc cref="LifecycleInfiniFrameWindowFeature.MarkAsClosed" />
    private void MarkAsClosed() => window.Features.Lifecycle.MarkAsClosed();

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

    private void RegisterNativeMilestoneCallbacks(IntPtr handle) {
        _milestoneRoot = GCHandle.Alloc(this);
        IntPtr context = GCHandle.ToIntPtr(_milestoneRoot);
        InfiniFrameNativeInteropStatus readyStatus = InfiniFrameNative.SetReadyCallback(handle, ReadyCallback, context);
        InfiniFrameNativeInteropStatus teardownStatus = InfiniFrameNative.SetTeardownCallback(handle, TeardownCallback, context);
        if (readyStatus == InfiniFrameNativeInteropStatus.Success
            && teardownStatus == InfiniFrameNativeInteropStatus.Success)
            return;

        throw new InfiniFrameNativeInteropException("Could not register native lifecycle milestone callbacks.");
    }

    private static void OnNativeReady(IntPtr context) {
        if (!TryGetLifecycle(context, out LifecycleInfiniFrameWindowFeature? lifecycle)) return;

        lifecycle.CompleteReady();
    }

    private static void OnNativeTeardown(IntPtr context) {
        if (!TryGetLifecycle(context, out LifecycleInfiniFrameWindowFeature? lifecycle)) return;

        // Complete outside the reverse P/Invoke so async disposal cannot release the
        // native instance while its teardown callback is still returning.
        ThreadPool.QueueUserWorkItem(callBack: static state => ((LifecycleInfiniFrameWindowFeature)state!).CompleteTeardown(), lifecycle);
    }

    private void CompleteReady() {
        window.MarkReady();
        _ready.TrySetResult();
    }

    private void CompleteTeardown() {
        window.MarkTeardownComplete();
        _teardown.TrySetResult();
        if (Volatile.Read(ref _disposed) != 0)
            CleanupClosedHandleAndCallbacks(true);
    }

    private void ReleaseMilestoneRootOnce() {
        if (Interlocked.Exchange(ref _milestoneRootReleased, 1) != 0) return;

        if (_milestoneRoot.IsAllocated) _milestoneRoot.Free();
    }

    private static bool TryGetLifecycle(
        IntPtr context,
        [NotNullWhen(true)]
        out LifecycleInfiniFrameWindowFeature? lifecycle
    ) {
        lifecycle = null;
        if (context == IntPtr.Zero) return false;

        try {
            lifecycle = GCHandle.FromIntPtr(context).Target as LifecycleInfiniFrameWindowFeature;
            return lifecycle is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}
