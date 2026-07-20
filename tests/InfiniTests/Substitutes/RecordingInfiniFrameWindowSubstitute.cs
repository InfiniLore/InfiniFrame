// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfiniTests.Substitutes;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class RecordingInfiniFrameWindowSubstitute {
    private readonly List<string> _sentWebMessages = [];
    #if NET9_0_OR_GREATER
    private readonly Lock _sentWebMessagesLock = new();
    #else
    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private readonly object _sentWebMessagesLock = new();
    #endif
    public IInfiniFrameWindow Window { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute() {
        Window = Substitute.For<IInfiniFrameWindow>();
        Window.LifecycleState.Returns(InfiniFrameWindowLifecycleState.Running);
        Window.ManagedThreadId.Returns(Environment.CurrentManagedThreadId);
        Window.Features.WebMessaging.SendWebMessageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask)
            .AndDoes(callInfo => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(callInfo.Arg<string>()!);
                }
            });
        Window.Features.WebMessaging.When(webMessaging => webMessaging.SendWebMessage(Arg.Any<string>()))
            .Do(callInfo => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(callInfo.Arg<string>()!);
                }
            });

        // Default wiring for simple tests that don't need explicit builder binding.
        var eventsStore = new InfiniFrameEventsStore();
        Window.Events.Returns(new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance));
        Window.EventsStore.Returns(eventsStore);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute BindToBuilder(IInfiniFrameWindowBuilder builder) {
        Window.Events.Returns(new InfiniFrameEvents(builder.EventsStore, NullLogger<InfiniFrameEvents>.Instance));
        Window.EventsStore.Returns(builder.EventsStore);
        return this;
    }

    public int CountEnvelopeMessagesById(string messageId) {
        List<string> snapshot;
        lock (_sentWebMessagesLock) {
            snapshot = [.._sentWebMessages];
        }

        return snapshot
            .Select(InteropEnvelopeProtocol.ParseIncomingMessage)
            .Count(result => result.IsSuccess && string.Equals(result.MessageId, messageId, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetSentMessagesSnapshot() {
        lock (_sentWebMessagesLock) {
            return [.._sentWebMessages];
        }
    }
}
