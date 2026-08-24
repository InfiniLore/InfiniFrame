// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging.Abstractions;

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
    private readonly Mock<IInfiniFrameWindow> _windowMock;

    public IInfiniFrameWindow Window { get; }
    public Mock<IInfiniFrameWindowFeatures> Features { get; }
    public Mock<IWebMessagingInfiniFrameWindowFeature> WebMessaging { get; }
    public Mock<ILifecycleInfiniFrameWindowFeature> Lifecycle { get; }
    public Mock<IStateInfiniFrameWindowFeature> State { get; }
    public Mock<IDecorationsInfiniFrameWindowFeature> Decorations { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute() {
        _windowMock = MockFactory.CreateWindowMock();
        Window = _windowMock.Object;
        Features = MockFactory.CreateFeaturesMock();
        WebMessaging = MockFactory.CreateWebMessagingMock();
        Lifecycle = MockFactory.CreateLifecycleMock();
        State = MockFactory.CreateStateMock();
        Decorations = MockFactory.CreateDecorationsMock();

        _windowMock.LifecycleState.Returns(InfiniFrameWindowLifecycleState.Running);
        _windowMock.ManagedThreadId.Returns(Environment.CurrentManagedThreadId);

        WebMessaging.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Callback((message, _) => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(message);
                }
            })
            .Returns(() => ValueTask.CompletedTask);
        WebMessaging.SendWebMessage(Any<string>())
            .Callback(message => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(message);
                }
            });
        Features.WebMessaging.Returns(WebMessaging.Object);
        Features.Lifecycle.Returns(Lifecycle.Object);
        Features.State.Returns(State.Object);
        Features.Decorations.Returns(Decorations.Object);
        _windowMock.Features.Returns(Features.Object);

        var eventsStore = new InfiniFrameEventsStore();
        _windowMock.Events.Returns(new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance));
        _windowMock.EventsStore.Returns(eventsStore);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute BindToBuilder(IInfiniFrameWindowBuilder builder) {
        _windowMock.Events.Returns(new InfiniFrameEvents(builder.EventsStore, NullLogger<InfiniFrameEvents>.Instance));
        _windowMock.EventsStore.Returns(builder.EventsStore);
        return this;
    }

    public int CountEnvelopeMessagesById(string messageId) {
        List<string> snapshot;
        lock (_sentWebMessagesLock) {
            snapshot = [.. _sentWebMessages];
        }

        return snapshot
            .Select(InteropEnvelopeProtocol.ParseIncomingMessage)
            .Count(result => result.IsSuccess && string.Equals(result.MessageId, messageId, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetSentMessagesSnapshot() {
        lock (_sentWebMessagesLock) {
            return [.. _sentWebMessages];
        }
    }
}
