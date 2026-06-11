// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
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
    IInfiniFrameWindowDebugging debugging,
    IInfiniFrameWindowConfiguration configuration,
    IServiceProvider? serviceProvider
) : IInfiniFrameWindow {
    private static readonly Lazy<IntPtr> LazyMainProgramHandle = new(NativeLibrary.GetMainProgramHandle);
    public IntPtr MainProgramHandle => LazyMainProgramHandle.Value;
    
    private IntPtr InstanceHandle { get; set; }
    IntPtr IInfiniFrameWindow.InstanceHandle {
        get => InstanceHandle;
        set => InstanceHandle = value;
    }

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
    
    public int ManagedThreadId { get; } = Environment.CurrentManagedThreadId;
    
    public Guid Id { get; } = Guid.NewGuid();
    
    public IInfiniFrameWindowConfiguration Configuration { get; } = configuration;
    public IInfiniFrameWindowDebugging Debugging { get; } = debugging;
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
    public IInfiniFrameEvents Events { get; } = events;
    public IInfiniFrameWindowFeatures Features { get; private set; } = null!;

    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void AssignFeatures(IInfiniFrameWindowFeatures features) {
        Features = features;
    }
}
