// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniWindowBuilder {
    bool UseDefaultLogger { get; set; }

    IInfiniWindowConfiguration Configuration { get; }
    IInfiniWindowEvents Events { get; }
    IInfiniWindowMessageHandlers MessageHandlers { get; }

    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }

    IInfiniWindow Build(IServiceProvider? provider = null);
}
