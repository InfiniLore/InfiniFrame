// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEvents {
    StaticAssetSettings? StaticAssets { get; set; }

    IInfiniFrameWindowNativeParameterBuilder Configuration { get; }
    IInfiniFrameWindowMessageHandler MessageHandlers { get; }
    IInfiniFrameWindowCustomSchemeHandlers CustomSchemeHandlers { get; }

    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
