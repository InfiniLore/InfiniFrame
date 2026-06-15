// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
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
    /// <inheritdoc cref="IInfiniFrameWindow.MainProgramHandle"/>
    public IntPtr MainProgramHandle => LazyMainProgramHandle.Value;
    
    private IntPtr InstanceHandle { get; set; }
    /// <inheritdoc cref="IInfiniFrameWindow.InstanceHandle"/>
    IntPtr IInfiniFrameWindow.InstanceHandle {
        get => InstanceHandle;
        set => InstanceHandle = value;
    }

    /// <inheritdoc cref="IInfiniFrameWindow.WindowHandle"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IntPtr WindowHandle {
        get {
            if (Features.Lifecycle.IsClosedOrClosing()) return IntPtr.Zero;
            
            IntPtr handle;
            if (OperatingSystem.IsWindows()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleWin32);
            else if (OperatingSystem.IsMacOS()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleMac);
            else if (OperatingSystem.IsLinux()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(logger, InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleLinux);
            else throw new PlatformNotSupportedException();

            return handle;
        }
    }
    
    /// <inheritdoc cref="IInfiniFrameWindow.ManagedThreadId"/>
    public int ManagedThreadId { get; } = Environment.CurrentManagedThreadId;
    
    /// <inheritdoc cref="IInfiniFrameWindow.Id"/>
    public Guid Id { get; } = Guid.NewGuid();
    
    /// <inheritdoc cref="IInfiniFrameWindow.Configuration"/>
    public IInfiniFrameWindowConfiguration Configuration { get; } = configuration;
    /// <inheritdoc cref="IInfiniFrameWindow.Debugging"/>
    public IInfiniFrameWindowFeatureDebugging Debugging => Features.Debugging;
    /// <inheritdoc/>
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
    /// <inheritdoc cref="IInfiniFrameWindow.Events"/>
    public IInfiniFrameEvents Events { get; } = events;
    /// <inheritdoc cref="IInfiniFrameWindow.Features"/>
    public IInfiniFrameWindowFeatures Features { get; private set; } = null!;

    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore"/>
    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void AssignFeatures(IInfiniFrameWindowFeatures features) {
        Features = features;
    }

    public void Dispose() {
        if (Features.Lifecycle.IsClosedOrClosing()) {
            Features.Lifecycle.CleanupNativeHandle();
            return;
        }

        Features.Lifecycle.Close();
        Features.Lifecycle.CleanupNativeHandle();
    }
}
