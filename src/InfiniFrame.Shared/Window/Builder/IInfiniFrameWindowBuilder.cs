// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEventsStore {
    IInfiniFrameStaticAssets? StaticAssets { get; set; }

    IInfiniFrameWindowBuilderConfiguration Configuration { get; }
    IInfiniFrameWindowBuilderFeatureDebugging Debugging { get; }

    IInfiniFrameWindowBuilderFeatures Features { get; }
    
    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
