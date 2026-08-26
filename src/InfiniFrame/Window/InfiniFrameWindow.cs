// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Runtime.InteropServices;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindow(
    ILogger<InfiniFrameWindow> logger,
    IInfiniFrameEvents events,
    IInfiniFrameWindowConfiguration configuration,
    IServiceProvider? serviceProvider
) : IInfiniFrameWindow, IDisposable, IAsyncDisposable {
    private static readonly Lazy<IntPtr> LazyMainProgramHandle = new(NativeLibrary.GetMainProgramHandle);
    private NativeWindowHandle? _instanceHandle;
    private int _lifecycleState = (int)InfiniFrameWindowLifecycleState.Created;
    private int _closeReturnState = (int)InfiniFrameWindowLifecycleState.Ready;
    private int _managedThreadId = Environment.CurrentManagedThreadId;
    private long _lastLifecycleTransitionUtcTicks = DateTimeOffset.UtcNow.UtcTicks;
    private int _asyncDisposing;
    private readonly object _diagnosticsLock = new();
    private readonly Dictionary<string, InfiniFrameOperationDiagnostics> _outstandingOperations = [];
    private InfiniFrameOperationDiagnostics? _lastOperation;
    private bool _ownsServiceProvider;
    #if NET9_0_OR_GREATER
    private readonly Lock _disposeLock = new();
    #else
    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    private readonly object _disposeLock = new();
    #endif
    /// <inheritdoc cref="IInfiniFrameWindow.MainProgramHandle" />
    public IntPtr MainProgramHandle => LazyMainProgramHandle.Value;

    public InfiniFrameWindowLifecycleState LifecycleState
        => (InfiniFrameWindowLifecycleState)Volatile.Read(ref _lifecycleState);

    /// <inheritdoc cref="IInfiniFrameWindow.WindowHandle" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IntPtr WindowHandle {
        get {
            InfiniFrameWindowLifecycleState state = LifecycleState;
            if (state is < InfiniFrameWindowLifecycleState.Creating or >= InfiniFrameWindowLifecycleState.CloseRequested)
                return IntPtr.Zero;

            try {
                if (OperatingSystem.IsWindows()) return NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, this, ManagedThreadId, InfiniFrameNative.GetWindowHandleWin32);
                if (OperatingSystem.IsMacOS()) return NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, this, ManagedThreadId, InfiniFrameNative.GetWindowHandleMac);
                if (OperatingSystem.IsLinux()) return NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, this, ManagedThreadId, InfiniFrameNative.GetWindowHandleLinux);

                throw new PlatformNotSupportedException();
            }
            catch (ObjectDisposedException) {
                return IntPtr.Zero;
            }
        }
    }

    /// <inheritdoc cref="IInfiniFrameWindow.ManagedThreadId" />
    public int ManagedThreadId => Volatile.Read(ref _managedThreadId);
    /// <inheritdoc cref="IInfiniFrameWindow.SetManagedThreadId" />
    void IInfiniFrameWindow.SetManagedThreadId(int managedThreadId) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(managedThreadId);
        Volatile.Write(ref _managedThreadId, managedThreadId);
    }

    /// <inheritdoc cref="IInfiniFrameWindow.Id" />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc cref="IInfiniFrameWindow.Configuration" />
    public IInfiniFrameWindowConfiguration Configuration { get; } = configuration;
    /// <inheritdoc cref="IInfiniFrameWindow.Debugging" />
    public IDebuggingInfiniFrameWindowFeature Debugging => Features.Debugging;
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc cref="IInfiniFrameWindow.Events" />
    public IInfiniFrameEvents Events { get; } = events;
    /// <inheritdoc cref="IInfiniFrameWindow.Features" />
    public IInfiniFrameWindowFeatures Features => _features ?? throw new InvalidOperationException("Features have not been assigned. Call AssignFeatures before accessing this property.");
    private IInfiniFrameWindowFeatures? _features;

    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore" />
    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void AssignFeatures(IInfiniFrameWindowFeatures features) {
        _features = features;
    }

    internal void SetOwnsServiceProvider(bool owns) {
        _ownsServiceProvider = owns;
    }

    internal string BeginDiagnosticOperation(string name, ulong id) {
        string key = $"{name}:{id}";
        lock (_diagnosticsLock) {
            _outstandingOperations[key] = new InfiniFrameOperationDiagnostics {
                Name = name,
                Id = id,
                StartedUtc = DateTimeOffset.UtcNow,
                FinalState = "Pending"
            };
        }

        return key;
    }

    internal void CompleteDiagnosticOperation(
        string? key,
        string finalState,
        int? nativeCode = null,
        string? failureReason = null
    ) {
        if (key is null) return;

        lock (_diagnosticsLock) {
            if (!_outstandingOperations.Remove(key, out InfiniFrameOperationDiagnostics? operation)) return;

            _lastOperation = operation with {
                CompletedUtc = DateTimeOffset.UtcNow,
                FinalState = finalState,
                NativeCode = nativeCode,
                FailureReason = failureReason
            };
        }
    }

    internal (DateTimeOffset TransitionUtc, IReadOnlyList<InfiniFrameOperationDiagnostics> Outstanding,
        InfiniFrameOperationDiagnostics? Last) GetOperationDiagnostics() {
        lock (_diagnosticsLock) {
            return (
                new DateTimeOffset(Volatile.Read(ref _lastLifecycleTransitionUtcTicks), TimeSpan.Zero),
                _outstandingOperations.Values.OrderBy(value => value.StartedUtc).ToArray(),
                _lastOperation
            );
        }
    }

    private void RecordLifecycleTransition()
        => Volatile.Write(ref _lastLifecycleTransitionUtcTicks, DateTimeOffset.UtcNow.UtcTicks);

    void IInfiniFrameWindow.BeginInitialization() {
        if (Interlocked.CompareExchange(ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.Initializing,
                (int)InfiniFrameWindowLifecycleState.Created) != (int)InfiniFrameWindowLifecycleState.Created) {
            throw new InvalidOperationException($"Cannot initialize a window in state {LifecycleState}.");
        }

        RecordLifecycleTransition();
    }

    void IInfiniFrameWindow.AssignNativeHandle(IntPtr handle) {
        ArgumentOutOfRangeException.ThrowIfZero(handle);
        NativeWindowHandle? safeHandle = new(handle);
        try {
            if (Interlocked.CompareExchange(ref _instanceHandle, safeHandle, null) is not null)
                throw new InvalidOperationException("A native handle is already assigned.");

            // Ownership transferred to _instanceHandle.
            safeHandle = null;
        }
        finally {
            safeHandle?.Dispose();
        }

        if (LifecycleState == InfiniFrameWindowLifecycleState.Creating) return;

        // A very early native closed callback won the transition. Keep ownership
        // for deferred teardown but never resurrect the window back to Running.
        if (LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return;

        Interlocked.Exchange(ref _instanceHandle, null).Dispose();
        throw new InvalidOperationException($"Cannot assign a native handle in state {LifecycleState}.");
    }

    void IInfiniFrameWindow.MarkReady() {
        if (Interlocked.CompareExchange(
                ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.Ready,
                (int)InfiniFrameWindowLifecycleState.Creating) != (int)InfiniFrameWindowLifecycleState.Creating
        ) return;

        RecordLifecycleTransition();
        // If not in Creating state, log but do not throw to avoid disrupting the lifecycle.
        // This may indicate an out-of-order lifecycle transition.
    }

    bool IInfiniFrameWindow.RequestClose() {
        while (true) {
            InfiniFrameWindowLifecycleState state = LifecycleState;
            if (state is not (InfiniFrameWindowLifecycleState.Creating or InfiniFrameWindowLifecycleState.Ready))
                return false;

            if (Interlocked.CompareExchange(ref _lifecycleState,
                    (int)InfiniFrameWindowLifecycleState.CloseRequested, (int)state) != (int)state)
                continue;

            // Write the return state atomically with the lifecycle transition so that
            // CancelCloseRequest always reads the value that corresponds to the current
            // CloseRequested transition.
            Volatile.Write(ref _closeReturnState, (int)state);
            RecordLifecycleTransition();
            return true;
        }
    }

    void IInfiniFrameWindow.CancelCloseRequest() {
        // Read the return state that was written by the most recent RequestClose.
        // Use an interlocked read on the lifecycle state to ensure we are cancelling
        // the same CloseRequested transition that produced this return state.
        int currentState = Volatile.Read(ref _lifecycleState);
        if (currentState != (int)InfiniFrameWindowLifecycleState.CloseRequested)
            return;

        int returnState = Volatile.Read(ref _closeReturnState);
        if (returnState is not ((int)InfiniFrameWindowLifecycleState.Creating or (int)InfiniFrameWindowLifecycleState.Ready))
            return;

        if (Interlocked.CompareExchange(ref _lifecycleState,
                returnState,
                (int)InfiniFrameWindowLifecycleState.CloseRequested) == (int)InfiniFrameWindowLifecycleState.CloseRequested)
            RecordLifecycleTransition();
    }

    void IInfiniFrameWindow.MarkNativeClosed() {
        while (true) {
            InfiniFrameWindowLifecycleState state = LifecycleState;
            if (state >= InfiniFrameWindowLifecycleState.NativeClosed) return;

            if (Interlocked.CompareExchange(ref _lifecycleState,
                    (int)InfiniFrameWindowLifecycleState.NativeClosed, (int)state) == (int)state) {
                RecordLifecycleTransition();
                return;
            }
        }
    }

    void IInfiniFrameWindow.MarkTeardownPending() {
        if (Interlocked.CompareExchange(ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.TeardownPending,
                (int)InfiniFrameWindowLifecycleState.NativeClosed) == (int)InfiniFrameWindowLifecycleState.NativeClosed)
            RecordLifecycleTransition();
    }

    void IInfiniFrameWindow.MarkTeardownComplete() {
        while (true) {
            InfiniFrameWindowLifecycleState state = LifecycleState;
            if (state >= InfiniFrameWindowLifecycleState.TeardownComplete) return;

            if (Interlocked.CompareExchange(ref _lifecycleState,
                    (int)InfiniFrameWindowLifecycleState.TeardownComplete, (int)state) == (int)state) {
                RecordLifecycleTransition();
                return;
            }
        }
    }

    void IInfiniFrameWindow.MarkNativeHandleReleased() {
        if (Interlocked.CompareExchange(ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.NativeHandleReleased,
                (int)InfiniFrameWindowLifecycleState.TeardownComplete) == (int)InfiniFrameWindowLifecycleState.TeardownComplete)
            RecordLifecycleTransition();
    }

    void IInfiniFrameWindow.MarkDisposed() {
        Volatile.Write(ref _lifecycleState, (int)InfiniFrameWindowLifecycleState.Disposed);
        RecordLifecycleTransition();
    }

    void IInfiniFrameWindow.ReleaseNativeHandle() {
        NativeWindowHandle? handle = Interlocked.Exchange(ref _instanceHandle, null);

        handle?.Dispose();
    }

    public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) {
        InfiniFrameWindowLifecycleState state = LifecycleState;
        bool allowed = access switch {
            NativeHandleAccess.Feature => state is InfiniFrameWindowLifecycleState.Creating or InfiniFrameWindowLifecycleState.Ready,
            NativeHandleAccess.Close => state is InfiniFrameWindowLifecycleState.Creating or InfiniFrameWindowLifecycleState.Ready or InfiniFrameWindowLifecycleState.CloseRequested,
            NativeHandleAccess.WaitForExit => state is InfiniFrameWindowLifecycleState.Creating or InfiniFrameWindowLifecycleState.Ready or InfiniFrameWindowLifecycleState.CloseRequested,
            _ => false
        };
        ObjectDisposedException.ThrowIf(!allowed, nameof(InfiniFrameWindow));

        NativeWindowHandle? handle = Volatile.Read(ref _instanceHandle);

        ObjectDisposedException.ThrowIf(handle is null, nameof(InfiniFrameWindow));
        return new NativeHandleLease(handle);
    }

    public void Dispose() {
        lock (_disposeLock) {
            if (LifecycleState == InfiniFrameWindowLifecycleState.Disposed) return;

            if (!Features.Lifecycle.IsClosedOrClosing()) {
                Features.Lifecycle.Close();
            }

            if (LifecycleState < InfiniFrameWindowLifecycleState.NativeClosed
                && Features.Lifecycle.CanWaitForCloseDuringDispose()) {
                Features.Lifecycle.WaitForClose();
            }

            if (LifecycleState < InfiniFrameWindowLifecycleState.TeardownComplete
                && Features.Lifecycle.CanWaitForTeardownDuringDispose()) {
                // Blocking here is safe: CanWaitForTeardownDuringDispose guarantees either the
                // message loop has already exited (non-owning thread or teardown complete) or we
                // are on a non-owning thread, so there is no risk of re-entrant UI deadlock.
                // For fully asynchronous disposal, prefer DisposeAsync() instead.
                Features.Lifecycle.WaitForTeardownAsync().AsTask().GetAwaiter().GetResult();
            }

            Features.Lifecycle.CleanupNativeHandle();

            if (_ownsServiceProvider && ServiceProvider is IDisposable disposableProvider) {
                disposableProvider.Dispose();
            }
        }
    }

    public async ValueTask DisposeAsync() {
        lock (_disposeLock) {
            if (Interlocked.CompareExchange(ref _asyncDisposing, 1, 0) != 0) return;
            if (LifecycleState == InfiniFrameWindowLifecycleState.Disposed) return;
        }

        try {
            if (!Features.Lifecycle.IsClosedOrClosing()) await Features.Lifecycle.CloseAsync().ConfigureAwait(false);
            await Features.Lifecycle.WaitForTeardownAsync().ConfigureAwait(false);
        }
        finally {
            Features.Lifecycle.CleanupNativeHandle();

            if (_ownsServiceProvider && ServiceProvider is IDisposable disposableProvider) {
                using IDisposable _ = disposableProvider;
            }
        }
    }
}
