// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowMessageHandlers : IInfiniFrameWindowMessageHandlers {
    private ConcurrentDictionary<string, Action<IInfiniFrameWindow, string?>> Handlers { get; } = new();
    public bool IsEmpty => Handlers.IsEmpty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterMessageHandler(string messageId, Action<IInfiniFrameWindow, string?> handler) {
        Handlers[messageId] = handler;
    }

    public void Handle(IInfiniFrameWindow window, string message) {
        if (window.MessageHandlers.IsEmpty) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        (string messageId, string? payload) = ParseMessage(message);

        if (!Handlers.TryGetValue(messageId, out Action<IInfiniFrameWindow, string?>? handler)) return;

        handler(window, payload);
    }

    private static (string messageId, string? payload) ParseMessage(string message) {
        string[] split = message.Split(';', 2, StringSplitOptions.RemoveEmptyEntries);
        return (split[0], split.ElementAtOrDefault(1));
    }
}
