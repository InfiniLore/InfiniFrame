// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniFrameTests.Shared.TestDoubles;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class RecordingInfiniFrameWindowSubstitute {
    private readonly List<string> _sentWebMessages = [];
    #if NET9_0_OR_GREATER
    private readonly Lock _sentWebMessagesLock = new();
    #else
    private readonly object _sentWebMessagesLock = new();
    #endif
    public IInfiniFrameWindow Window { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute() {
        Window = Substitute.For<IInfiniFrameWindow>();
        Window.Logger.Returns(NullLogger<IInfiniFrameWindow>.Instance);
        Window.ManagedThreadId.Returns(Environment.CurrentManagedThreadId);
        Window.SendWebMessageAsync(Arg.Any<string>())
            .Returns(Task.CompletedTask)
            .AndDoes(callInfo => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(callInfo.Arg<string>());
                }
            });
        Window.When(window => window.SendWebMessage(Arg.Any<string>()))
            .Do(callInfo => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(callInfo.Arg<string>());
                }
            });

        // Default wiring for simple tests that don't need explicit builder binding.
        Window.Events.Returns(new InfiniFrameWindowEvents());
        Window.MessageHandlers.Returns(new InfiniFrameWindowMessageHandlers());
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute BindToBuilder(IInfiniFrameWindowBuilder builder) {
        Window.Events.Returns(builder.Events);
        Window.MessageHandlers.Returns(builder.MessageHandlers);
        return this;
    }

    public int CountEnvelopeMessagesById(string messageId) {
        List<string> snapshot;
        lock (_sentWebMessagesLock) {
            snapshot = [.._sentWebMessages];
        }

        return snapshot
            .Select(InteropEnvelopeProtocol.ParseIncomingMessage)
            .Count(result => result.Success && string.Equals(result.MessageId, messageId, StringComparison.Ordinal));
    }
}
