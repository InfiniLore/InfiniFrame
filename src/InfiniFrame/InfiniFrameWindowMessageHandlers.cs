// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Interop;
using InfiniFrame.Js.Interop;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowMessageHandlers : IInfiniFrameWindowMessageHandlers {
    public bool IsEmpty => Handlers.IsEmpty;

    private ConcurrentDictionary<string, Action<IInfiniFrameWindow, string?>> Handlers { get; } = new();
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterMessageHandler(string messageId, Action<IInfiniFrameWindow, string?> handler) {
        Handlers.AddOrUpdate(messageId, handler, (_, _) => handler);
    }

    public void Handle(IInfiniFrameWindow window, string message) {
        if (window.MessageHandlers.IsEmpty) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (!parseResult.Success) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!Handlers.TryGetValue(messageId, out Action<IInfiniFrameWindow, string?>? handler)) return;

        handler(window, payload);
    }
    
    internal InfiniFrameWindowMessageHandlersSnapshot ToSnapshot()
        => new(Handlers.ToArray());

    internal static InfiniFrameWindowMessageHandlers FromSnapshot(InfiniFrameWindowMessageHandlersSnapshot snapshot) {
        var copy = new InfiniFrameWindowMessageHandlers();

        foreach ((string key, Action<IInfiniFrameWindow, string?> value) in snapshot.Handlers) {
            copy.RegisterMessageHandler(key, value);
        }

        return copy;
    }
}
