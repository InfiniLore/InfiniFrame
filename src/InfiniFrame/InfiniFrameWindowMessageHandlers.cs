// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Interop;
using InfiniFrame.Js.Interop;
using Microsoft.Extensions.Logging;
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
        Handlers.AddOrUpdate(messageId, handler, updateValueFactory: (_, _) => handler);
    }

    public void Handle(IInfiniFrameWindow window, string message) {
        if (IsEmpty) return;
        if (string.IsNullOrWhiteSpace(message)) return;

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (!parseResult.Success) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!Handlers.TryGetValue(messageId, out Action<IInfiniFrameWindow, string?>? handler)) return;

        try {
            handler(window, payload);
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing web message '{MessageId}'", messageId);
        }
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

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
