// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Handles;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the main InfiniFrame window and provides access to its configuration, features, and events.
/// </summary>
public interface IInfiniFrameWindow : IHasInfiniFrameEventsStore, INativeWindowHandleOwner {
    /// <summary>
    ///     Gets the service provider associated with this window.
    /// </summary>
    internal IServiceProvider? ServiceProvider { get; }

    /// <summary>
    ///     Gets the events manager for handling window lifecycle and user interaction events.
    /// </summary>
    IInfiniFrameEvents Events { get; }

    /// <summary>
    ///     Gets the debugging feature for the window.
    /// </summary>
    IDebuggingInfiniFrameWindowFeature Debugging { get; }

    /// <summary>
    ///     Gets the configuration for the window.
    /// </summary>
    IInfiniFrameWindowConfiguration Configuration { get; }

    /// <summary>
    ///     Gets the collection of features available on this window.
    /// </summary>
    IInfiniFrameWindowFeatures Features { get; }

    /// <summary>
    ///     Gets the main program handle for the application.
    /// </summary>
    IntPtr MainProgramHandle { get; }

    /// <summary>Gets the current window lifecycle state.</summary>
    InfiniFrameWindowLifecycleState LifecycleState { get; }

    /// <summary>
    ///     Gets the native window handle.
    /// </summary>
    IntPtr WindowHandle { get; }

    /// <summary>
    ///     Gets the managed thread ID that owns window invoke dispatching.
    /// </summary>
    int ManagedThreadId { get; }

    /// <summary>
    ///     Gets the unique identifier for this window instance.
    /// </summary>
    Guid Id { get; }

    internal void BeginInitialization();
    internal void AssignNativeHandle(IntPtr handle);
    internal void MarkReady();
    internal bool RequestClose();
    internal void CancelCloseRequest();
    internal void MarkNativeClosed();
    internal void MarkTeardownPending();
    internal void MarkTeardownComplete();
    internal void MarkNativeHandleReleased();
    internal void MarkDisposed();
    internal void ReleaseNativeHandle();
    internal void MarkNativeHandleSafeToDestroy();

    /// <summary>
    ///     Updates the managed thread ID used for invoke dispatching.
    /// </summary>
    internal void SetManagedThreadId(int managedThreadId);
}
