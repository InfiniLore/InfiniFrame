// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameWindowEventsStore {
    StaticAssetSettings? StaticAssets { get; set; }

    IInfiniFrameWindowNativeParameterBuilder Configuration { get; }

    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
