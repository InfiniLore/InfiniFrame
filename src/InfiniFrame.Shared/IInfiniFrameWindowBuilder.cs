// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEvents {
    bool UseDefaultLogger { get; set; }
    StaticAssetSettings? StaticAssets { get; set; }

    IInfiniFrameWindowNativeParameterBuilder Configuration { get; }
    IInfiniFrameWindowMessageHandlers MessageHandlers { get; }

    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }

    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
