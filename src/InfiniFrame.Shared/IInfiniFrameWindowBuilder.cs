// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEvents {
    bool UseDefaultLogger { get; set; }

    IInfiniFrameWindowConfiguration Configuration { get; }
    IInfiniFrameWindowMessageHandlers MessageHandlers { get; }

    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
