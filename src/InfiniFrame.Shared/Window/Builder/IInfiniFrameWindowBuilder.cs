// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEventsStore {
    IInfiniFrameStaticAssets? StaticAssets { get; set; }

    IInfiniFrameOptionsBuilder Configuration { get; }
    IInfiniFrameWindowDebuggingBuilder Debugging { get; }

    IInfiniFrameWindowBuilderFeatures Features { get; }
    
    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
