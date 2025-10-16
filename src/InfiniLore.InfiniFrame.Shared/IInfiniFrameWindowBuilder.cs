// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilder {
    bool UseDefaultLogger { get; set; }

    IInfiniFrameWindowConfiguration Configuration { get; }
    IInfiniFrameWindowEvents Events { get; }
    IInfiniFrameWindowMessageHandlers MessageHandlers { get; }

    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }

    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
