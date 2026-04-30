// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowMessageHandler {
    bool IsEmpty { get; }
    int Count { get; }

    void RegisterHandler(string messageId, Action<IInfiniFrameWindow, string?> handler);
    void RegisterHandler(string messageId, Func<IInfiniFrameWindow, string?, string?> handler);
    
    bool TryHandlePostDataRequest(IInfiniFrameWindow sender, string message);
    bool TryHandleGetDataRequest(IInfiniFrameWindow window, string message, out string? response);
}
