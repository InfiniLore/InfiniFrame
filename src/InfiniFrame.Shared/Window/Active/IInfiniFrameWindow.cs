// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindow : IHasInfiniFrameEventsStore {
    internal IServiceProvider? ServiceProvider { get; }
    IInfiniFrameEvents Events { get; }
    IInfiniFrameWindowFeatureDebugging Debugging { get; }
    IInfiniFrameWindowConfiguration Configuration { get; }
    IInfiniFrameWindowFeatures Features { get; }
    
    IntPtr MainProgramHandle { get; }
    IntPtr InstanceHandle { get; internal set; }
    IntPtr WindowHandle { get; }
    int ManagedThreadId { get; }

    Guid Id { get; }
}
