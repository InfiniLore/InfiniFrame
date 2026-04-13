// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowCustomSchemeHandlers {
    bool IsEmpty { get; }
    int Length { get; }

    void RegisterCustomSchemeHandler(string messageId, NetCustomSchemeDelegate handler);
    IEnumerable<(string, NetCustomSchemeDelegate)> GetRegisteredHandlers();
    bool TryGetHandler(string scheme, out NetCustomSchemeDelegate? netCustomSchemeDelegate);
    bool ContainsCustomSchemeHandler(string schemeName);
}
