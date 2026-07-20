// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class EventExceptionPolicyTests {
    [Test]
    public async Task OrderedResultEvent_HandlerException_PropagatesAndStopsDispatch(CancellationToken ct = default) {
        var eventSource = new OrderedResultEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        int invoked = 0;
        eventSource.Add((_, _) => throw new InvalidOperationException("expected"));
        eventSource.Add((_, _) => ++invoked);

        await Assert.That(() => eventSource.Invoke(window, "payload"))
            .Throws<InvalidOperationException>();
        await Assert.That(invoked).IsEqualTo(0);
    }

    [Test]
    public async Task KeyedEvent_HandlerException_Propagates(CancellationToken ct = default) {
        var eventSource = new KeyedEvent<string, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        eventSource.Add("key", handler: (_, _) => throw new InvalidOperationException("expected"));

        await Assert.That(() => eventSource.TryInvoke("key", window, "payload"))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task KeyedResultEvent_NullResult_IsAHandledRequest(CancellationToken ct = default) {
        var eventSource = new KeyedResultEvent<string, string, string?>();
        var window = Substitute.For<IInfiniFrameWindow>();
        eventSource.Add("key", handler: static (_, _) => null);

        bool handled = eventSource.TryInvoke("key", window, "payload", out string? result);

        await Assert.That(handled).IsTrue();
        await Assert.That(result).IsNull();
    }
}
