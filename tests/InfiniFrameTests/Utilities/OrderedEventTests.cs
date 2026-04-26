// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Immutable;

namespace InfiniFrameTests.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OrderedEventTests {
    private static InfiniFrameWindow CreateWindow() {
        var events = new InfiniFrameWindowEvents();
        var window = new InfiniFrameWindow {
            Logger = NullLogger<IInfiniFrameWindow>.Instance,
            ServiceProvider = null,
            CustomSchemes = new InfiniFrameWindowCustomSchemeHandlers(),
            Parent = null,
            Events = events,
            MessageHandlers = new InfiniFrameWindowMessageHandler()
        };
        events.CompleteSetup(window);
        return window;
    }

    [Test]
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(OrderedEvent_InvokesInRegistrationOrder)}")]
    public async Task OrderedEvent_InvokesInRegistrationOrder() {
        // Arrange
        var orderedEvent = new InfiniFrameOrderedEvent();
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
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(OrderedEvent_RemoveStopsInvocation)}")]
    public async Task OrderedEvent_RemoveStopsInvocation() {
        // Arrange
        var orderedEvent = new InfiniFrameOrderedEvent();
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
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(OrderedEvent_SnapshotIsImmutable)}")]
    public async Task OrderedEvent_SnapshotIsImmutable() {
        // Arrange
        var orderedEvent = new InfiniFrameOrderedEvent();
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
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(OrderedEvent_OperatorsAddAndRemove)}")]
    public async Task OrderedEvent_OperatorsAddAndRemove() {
        // Arrange
        var orderedEvent = new InfiniFrameOrderedEvent();
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
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(OrderedEventWithPayload_InvokesWithPayload)}")]
    public async Task OrderedEventWithPayload_InvokesWithPayload() {
        // Arrange
        var orderedEvent = new InfiniFrameOrderedEvent<int>();
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
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(ClosingEvent_ReturnsLastResult)}")]
    public async Task ClosingEvent_ReturnsLastResult() {
        // Arrange
        var closingEvent = new InfiniFrameOrderedClosingEvent();
        InfiniFrameWindow window = CreateWindow();

        closingEvent.Add((_, _) => false);
        closingEvent.Add((_, _) => true);

        // Act
        bool? result = closingEvent.Invoke(window);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(OrderedEventTests)}.{nameof(ClosingEvent_ReturnsNullWhenEmpty)}")]
    public async Task ClosingEvent_ReturnsNullWhenEmpty() {
        // Arrange
        var closingEvent = new InfiniFrameOrderedClosingEvent();
        InfiniFrameWindow window = CreateWindow();

        // Act
        bool? result = closingEvent.Invoke(window);

        // Assert
        await Assert.That(result).IsNull();
    }
}
