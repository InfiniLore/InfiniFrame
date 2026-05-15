// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Collections.Immutable;

namespace InfiniFrameTests.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OrderedEventTests {
    private static InfiniFrameWindow CreateWindow() {
        var eventStore = new InfiniFrameEventsStore();
        var events = new InfiniFrameEvents(eventStore);
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider = null,
            Events = events,
            Configuration = Substitute.For<IInfiniFrameOptions>(),
            StaticAssets = null,
        };
        var nativeParameters = default(InfiniFrameNativeParameters);    
        
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window);
        
        return window;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task OrderedEvent_InvokesInRegistrationOrder(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        InfiniFrameWindow window = CreateWindow();
        var calls = new List<int>();

        orderedEvent.Add(_ => calls.Add(1));
        orderedEvent.Add(_ => calls.Add(2));

        // Act
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(calls.Count).IsEqualTo(2);
        await Assert.That(calls[0]).IsEqualTo(1);
        await Assert.That(calls[1]).IsEqualTo(2);
    }

    [Test]
    public async Task OrderedEvent_RemoveStopsInvocation(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        InfiniFrameWindow window = CreateWindow();
        var calls = new List<int>();
        Action<IInfiniFrameWindow> handler = _ => calls.Add(1);

        orderedEvent.Add(handler);
        orderedEvent.Remove(handler);

        // Act
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(calls.Count).IsEqualTo(0);
    }

    [Test]
    public async Task OrderedEvent_SnapshotIsImmutable(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        Action<IInfiniFrameWindow> handler1 = _ => { };
        Action<IInfiniFrameWindow> handler2 = _ => { };

        orderedEvent.Add(handler1);
        ImmutableArray<Action<IInfiniFrameWindow>> snapshot = orderedEvent.Snapshot;

        // Act
        orderedEvent.Add(handler2);

        // Assert
        await Assert.That(snapshot.Length).IsEqualTo(1);
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(2);
    }

    [Test]
    public async Task OrderedEvent_OperatorsAddAndRemove(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        InfiniFrameWindow window = CreateWindow();
        var calls = new List<int>();
        Action<IInfiniFrameWindow> handler = _ => calls.Add(1);

        // Act
        orderedEvent.Add(handler);
        orderedEvent.Invoke(window);
        orderedEvent.Remove(handler);
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(calls.Count).IsEqualTo(1);
    }

    [Test]
    public async Task OrderedEventWithPayload_InvokesWithPayload(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        InfiniFrameWindow window = CreateWindow();
        var calls = new List<int>();

        orderedEvent.Add((_, value) => calls.Add(value));
        orderedEvent.Add((_, value) => calls.Add(value + 1));

        // Act
        orderedEvent.Invoke(window, 10);

        // Assert
        await Assert.That(calls.Count).IsEqualTo(2);
        await Assert.That(calls[0]).IsEqualTo(10);
        await Assert.That(calls[1]).IsEqualTo(11);
    }

    [Test]
    public async Task ClosingEvent_ReturnsLastResult(CancellationToken ct = default) {
        // Arrange
        var closingEvent = new OrderedResultEvent<EventArgs?, bool>();
        InfiniFrameWindow window = CreateWindow();

        closingEvent.Add((_, _) => false);
        closingEvent.Add((_, _) => true);

        // Act
        bool[] result = closingEvent.Invoke(window, EventArgs.Empty);

        // Assert
        await Assert.That(result).Count().IsEqualTo(2);
        await Assert.That(result.First()).IsFalse();
        await Assert.That(result.Last()).IsTrue();
    }

    [Test]
    public async Task ClosingEvent_ReturnsNullWhenEmpty(CancellationToken ct = default) {
        // Arrange
        var closingEvent = new OrderedResultEvent<EventArgs?, bool>();
        InfiniFrameWindow window = CreateWindow();

        // Act
        bool[] result = closingEvent.Invoke(window, EventArgs.Empty);

        // Assert
        await Assert.That(result).IsEmpty();
    }
}
