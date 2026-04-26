// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Concurrent;

namespace InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameGetMessageService : IInfiniFrameGetMessageService {
    private readonly ConcurrentDictionary<string, Func<IInfiniFrameWindow, string?, string?>> _handlers = new();

    public void RegisterHandler(string messageId, Func<IInfiniFrameWindow, string?, string?> handler) {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.AddOrUpdate(messageId, handler, static (_, updated) => updated);
    }

    public bool TryHandle(IInfiniFrameWindow window, string messageId, string? payload, out string? response) {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentException.ThrowIfNullOrWhiteSpace(messageId);

        if (!_handlers.TryGetValue(messageId, out Func<IInfiniFrameWindow, string?, string?>? handler)) {
            response = null;
            return false;
        }

        response = handler(window, payload);
        return true;
    }
}
