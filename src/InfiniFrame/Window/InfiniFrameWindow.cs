// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindow(
    ILogger<InfiniFrameWindow> logger,
    IInfiniFrameEvents events,
    IInfiniFrameWindowConfiguration configuration,
    IServiceProvider? serviceProvider
) : IInfiniFrameWindow, IDisposable {
    private static readonly Lazy<IntPtr> LazyMainProgramHandle = new(NativeLibrary.GetMainProgramHandle);
    private NativeWindowHandle? _instanceHandle;
    private int _lifecycleState = (int)InfiniFrameWindowLifecycleState.Created;
    private int _managedThreadId = Environment.CurrentManagedThreadId;
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
            if (LifecycleState != InfiniFrameWindowLifecycleState.Running) return IntPtr.Zero;

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
    public IInfiniFrameWindowFeatureDebugging Debugging => Features.Debugging;
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc cref="IInfiniFrameWindow.Events" />
    public IInfiniFrameEvents Events { get; } = events;
    /// <inheritdoc cref="IInfiniFrameWindow.Features" />
    public IInfiniFrameWindowFeatures Features { get; private set; } = null!;

    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore" />
    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void AssignFeatures(IInfiniFrameWindowFeatures features) {
        Features = features;
    }

    void IInfiniFrameWindow.BeginInitialization() {
        if (Interlocked.CompareExchange(ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.Initializing,
                (int)InfiniFrameWindowLifecycleState.Created) != (int)InfiniFrameWindowLifecycleState.Created) {
            throw new InvalidOperationException($"Cannot initialize a window in state {LifecycleState}.");
        }
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

        if (Interlocked.CompareExchange(ref _lifecycleState,
                (int)InfiniFrameWindowLifecycleState.Running,
                (int)InfiniFrameWindowLifecycleState.Initializing) == (int)InfiniFrameWindowLifecycleState.Initializing)
            return;

        // A very early native closed callback won the transition. Keep ownership
        // for deferred teardown but never resurrect the window back to Running.
        if (LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) return;

        Interlocked.Exchange(ref _instanceHandle, null).Dispose();
        throw new InvalidOperationException($"Cannot assign a native handle in state {LifecycleState}.");
    }

    bool IInfiniFrameWindow.RequestClose()
        => Interlocked.CompareExchange(ref _lifecycleState,
            (int)InfiniFrameWindowLifecycleState.ClosingRequested,
            (int)InfiniFrameWindowLifecycleState.Running) == (int)InfiniFrameWindowLifecycleState.Running;

    void IInfiniFrameWindow.MarkNativeClosed() {
        while (true) {
            InfiniFrameWindowLifecycleState state = LifecycleState;
            if (state >= InfiniFrameWindowLifecycleState.NativeClosed) return;
            if (Interlocked.CompareExchange(ref _lifecycleState,
                    (int)InfiniFrameWindowLifecycleState.NativeClosed, (int)state) == (int)state) return;
        }
    }

    void IInfiniFrameWindow.MarkDisposed()
        => Volatile.Write(ref _lifecycleState, (int)InfiniFrameWindowLifecycleState.Disposed);

    void IInfiniFrameWindow.ReleaseNativeHandle() {
        NativeWindowHandle? handle = Interlocked.Exchange(ref _instanceHandle, null);

        handle?.Dispose();
    }

    public NativeHandleLease AcquireNativeHandle(NativeHandleAccess access = NativeHandleAccess.Feature) {
        InfiniFrameWindowLifecycleState state = LifecycleState;
        bool allowed = access switch {
            NativeHandleAccess.Feature => state == InfiniFrameWindowLifecycleState.Running,
            NativeHandleAccess.Close => state is InfiniFrameWindowLifecycleState.Running or InfiniFrameWindowLifecycleState.ClosingRequested,
            NativeHandleAccess.WaitForExit => state is InfiniFrameWindowLifecycleState.Running or InfiniFrameWindowLifecycleState.ClosingRequested,
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
        }

        if (!Features.Lifecycle.IsClosedOrClosing()) {
            Features.Lifecycle.Close();
        }

        if (LifecycleState < InfiniFrameWindowLifecycleState.NativeClosed
            && Features.Lifecycle.CanWaitForCloseDuringDispose()) {
            Features.Lifecycle.WaitForClose();
        }

        Features.Lifecycle.CleanupNativeHandle();
    }
}
