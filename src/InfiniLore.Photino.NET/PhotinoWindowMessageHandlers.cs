// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoWindowMessageHandlers : IPhotinoWindowMessageHandlers {
    private Dictionary<string, Action<IPhotinoWindow, string?>> Handlers { get; } = new Dictionary<string, Action<IPhotinoWindow, string?>>();
    public bool IsEmpty => Handlers.Count == 0;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Register(string messageId, Action<IPhotinoWindow, string?> handler) {
        Handlers.Add(messageId, handler);   
    }
    
    public void Handle(object? sender, string? message) {
        if (sender is not IPhotinoWindow window) return;
        if (window.MessageHandlers.IsEmpty) return;
        if (string.IsNullOrWhiteSpace(message)) return;
        
        (string messageId, string? payload) = ParseMessage(message);
        
        if (!Handlers.TryGetValue(messageId, out Action<IPhotinoWindow, string?>? handler)) return;
        
        handler(window, payload);
    }
    
    private static (string messageId, string? payload) ParseMessage(string message) {
        string[] split = message.Split(';', 2, StringSplitOptions.RemoveEmptyEntries);
        return (split[0], split.ElementAtOrDefault(1));
    }
}
