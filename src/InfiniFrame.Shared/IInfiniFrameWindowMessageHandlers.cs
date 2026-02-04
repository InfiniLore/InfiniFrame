// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowMessageHandlers {
    bool IsEmpty { get; }

    void RegisterMessageHandler(string messageId, Action<IInfiniFrameWindow, string?> handler);
    void Handle(IInfiniFrameWindow sender, string message);
}
