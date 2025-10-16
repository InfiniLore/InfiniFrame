// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoWindowMessageHandlers : IInfiniWindowMessageHandlers {
    private Dictionary<string, Action<IInfiniWindow, string?>> Handlers { get; } = new();
    public bool IsEmpty => Handlers.Count == 0;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterMessageHandler(string messageId, Action<IInfiniWindow, string?> handler) {
        Handlers.Add(messageId, handler);
    }

    public void Handle(object? sender, string? message) {
        if (sender is not IInfiniWindow window) return;
        if (window.MessageHandlers.IsEmpty) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        (string messageId, string? payload) = ParseMessage(message);

        if (!Handlers.TryGetValue(messageId, out Action<IInfiniWindow, string?>? handler)) return;

        handler(window, payload);
    }

    private static (string messageId, string? payload) ParseMessage(string message) {
        string[] split = message.Split(';', 2, StringSplitOptions.RemoveEmptyEntries);
        return (split[0], split.ElementAtOrDefault(1));
    }
}
