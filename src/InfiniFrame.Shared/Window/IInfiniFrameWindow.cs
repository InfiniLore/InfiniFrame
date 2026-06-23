// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the main InfiniFrame window and provides access to its configuration, features, and events.
/// </summary>
public interface IInfiniFrameWindow : IHasInfiniFrameEventsStore {
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
    IInfiniFrameWindowFeatureDebugging Debugging { get; }
    
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
    
    /// <summary>
    ///     Gets or sets the native instance handle for the window.
    /// </summary>
    IntPtr InstanceHandle { get; internal set; }
    
    /// <summary>
    ///     Gets the native window handle.
    /// </summary>
    IntPtr WindowHandle { get; }
    
    /// <summary>
    ///     Gets the managed thread ID that owns window invoke dispatching.
    /// </summary>
    int ManagedThreadId { get; }

    /// <summary>
    ///     Updates the managed thread ID used for invoke dispatching.
    /// </summary>
    internal void SetManagedThreadId(int managedThreadId);
    
    /// <summary>
    ///     Gets the unique identifier for this window instance.
    /// </summary>
    Guid Id { get; }
}
