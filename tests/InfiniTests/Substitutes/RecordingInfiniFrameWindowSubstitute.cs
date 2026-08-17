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
    private readonly Mock<IInfiniFrameWindowFeatures> _featuresMock;
    private readonly Mock<IWebMessagingInfiniFrameWindowFeature> _webMessagingMock;
    private readonly Mock<ILifecycleInfiniFrameWindowFeature> _lifecycleMock;
    private readonly Mock<IStateInfiniFrameWindowFeature> _stateMock;
    private readonly Mock<IDecorationsInfiniFrameWindowFeature> _decorationsMock;

    public IInfiniFrameWindow Window { get; }
    public Mock<IInfiniFrameWindowFeatures> Features => _featuresMock;
    public Mock<IWebMessagingInfiniFrameWindowFeature> WebMessaging => _webMessagingMock;
    public Mock<ILifecycleInfiniFrameWindowFeature> Lifecycle => _lifecycleMock;
    public Mock<IStateInfiniFrameWindowFeature> State => _stateMock;
    public Mock<IDecorationsInfiniFrameWindowFeature> Decorations => _decorationsMock;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public RecordingInfiniFrameWindowSubstitute() {
        _windowMock = MockFactory.CreateWindowMock();
        Window = _windowMock.Object;
        _featuresMock = MockFactory.CreateFeaturesMock();
        _webMessagingMock = MockFactory.CreateWebMessagingMock();
        _lifecycleMock = MockFactory.CreateLifecycleMock();
        _stateMock = MockFactory.CreateStateMock();
        _decorationsMock = MockFactory.CreateDecorationsMock();

        _windowMock.LifecycleState.Returns(InfiniFrameWindowLifecycleState.Running);
        _windowMock.ManagedThreadId.Returns(Environment.CurrentManagedThreadId);

        _webMessagingMock.SendWebMessageAsync(Any<string>(), Any<CancellationToken>())
            .Callback((message, _) => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(message);
                }
            })
            .Returns(() => ValueTask.CompletedTask);
        _webMessagingMock.SendWebMessage(Any<string>())
            .Callback(message => {
                lock (_sentWebMessagesLock) {
                    _sentWebMessages.Add(message);
                }
            });
        _featuresMock.WebMessaging.Returns(_webMessagingMock.Object);
        _featuresMock.Lifecycle.Returns(_lifecycleMock.Object);
        _featuresMock.State.Returns(_stateMock.Object);
        _featuresMock.Decorations.Returns(_decorationsMock.Object);
        _windowMock.Features.Returns(_featuresMock.Object);

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
