// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEventsStore {
    StaticAssetSettings? StaticAssets { get; set; }

    IInfiniFrameOptionsBuilder Configuration { get; }

    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
