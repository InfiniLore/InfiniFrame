// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEvents {
    bool UseDefaultLogger { get; set; }
    StaticAssetSettings? StaticAssets { get; set; }

    IInfiniFrameWindowConfiguration Configuration { get; }
    IInfiniFrameWindowMessageHandlers MessageHandlers { get; }

    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }

    [RequiresUnreferencedCode("Configuration binding uses reflection and may require preserved members for trimming.")]
    [RequiresDynamicCode("Configuration binding may require runtime code generation under NativeAOT.")]
    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
