// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowMessageHandlers {
    bool IsEmpty { get; }

    void RegisterMessageHandler(string messageId, Action<IInfiniFrameWindow, string?> handler);
    void Handle(object? sender, string? message);
}
