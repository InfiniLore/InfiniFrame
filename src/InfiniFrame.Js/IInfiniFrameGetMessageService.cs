// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameGetMessageService {
    void RegisterHandler(string messageId, Func<IInfiniFrameWindow, string?, string?> handler);
    bool TryHandle(IInfiniFrameWindow window, string messageId, string? payload, out string? response);
}
