// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OrderedEventTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Add
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();

        // Act & Assert
        await Assert.That(() => orderedEvent.Add(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_SingleHandler_SnapshotContainsOneEntry(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        Action<IInfiniFrameWindow> handler = _ => {};

        // Act
        orderedEvent.Add(handler);

        // Assert
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(1);
    }

    [Test]
    public async Task Add_SameHandlerTwice_AppendsBothEntries(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        Action<IInfiniFrameWindow> handler = _ => {};

        // Act
        orderedEvent.Add(handler);
        orderedEvent.Add(handler);

        // Assert
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Remove
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Remove_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();

        // Act & Assert
        await Assert.That(() => orderedEvent.Remove(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Remove_RegisteredHandler_ReducesSnapshotCount(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        Action<IInfiniFrameWindow> handler = _ => {};
        orderedEvent.Add(handler);

        // Act
        orderedEvent.Remove(handler);

        // Assert
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(0);
    }

    [Test]
    public async Task Remove_HandlerNotRegistered_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        Action<IInfiniFrameWindow> unregistered = _ => {};

        // Act & Assert, removing a handler that was never added must not throw
        await Assert.That(() => orderedEvent.Remove(unregistered)).ThrowsNothing();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Invoke
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Invoke_NoHandlers_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        var window = MockFactory.CreateWindowMock().Object;

        // Act & Assert
        await Assert.That(() => orderedEvent.Invoke(window)).ThrowsNothing();
    }

    [Test]
    public async Task Invoke_SingleHandler_PassesWindowToHandler(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        var window = MockFactory.CreateWindowMock().Object;
        IInfiniFrameWindow? received = null;
        orderedEvent.Add(w => received = w);

        // Act
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(received).IsEqualTo(window);
    }

    [Test]
    public async Task Invoke_MultipleHandlers_InvokesInRegistrationOrder(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        var window = MockFactory.CreateWindowMock().Object;
        var calls = new List<int>();

        orderedEvent.Add(_ => calls.Add(1));
        orderedEvent.Add(_ => calls.Add(2));
        orderedEvent.Add(_ => calls.Add(3));

        // Act
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(calls).IsEquivalentTo([1, 2, 3]);
    }

    [Test]
    public async Task Invoke_AfterRemove_DoesNotCallRemovedHandler(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        var window = MockFactory.CreateWindowMock().Object;
        var calls = new List<int>();
        Action<IInfiniFrameWindow> first = _ => calls.Add(1);
        Action<IInfiniFrameWindow> second = _ => calls.Add(2);

        orderedEvent.Add(first);
        orderedEvent.Add(second);
        orderedEvent.Remove(first);

        // Act
        orderedEvent.Invoke(window);

        // Assert
        await Assert.That(calls).IsEquivalentTo([2]);
    }

    [Test]
    public async Task Invoke_HandlerThrowsException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();
        var window = MockFactory.CreateWindowMock().Object;
        orderedEvent.Add(_ => throw new InvalidOperationException("boom"));

        // Act & Assert, OrderedEvent.Invoke does not swallow exceptions
        await Assert.That(() => orderedEvent.Invoke(window)).Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Snapshot
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Snapshot_IsImmutable_SubsequentAddDoesNotAffectCapturedSnapshot(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent();

        orderedEvent.Add(Action);

        // Act
        ImmutableArray<Action<IInfiniFrameWindow>> snapshot = orderedEvent.Snapshot;
        orderedEvent.Add(Action);

        // Assert, the captured snapshot must not reflect the later add
        await Assert.That(snapshot.Length).IsEqualTo(1);
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(2);
        return;

        void Action(IInfiniFrameWindow _) {}
    }

    [Test]
    public async Task Snapshot_StartsEmpty(CancellationToken ct = default) {
        // Arrange & Act
        var orderedEvent = new OrderedEvent();

        // Assert
        await Assert.That(orderedEvent.Snapshot.ToArray()).IsEmpty();
    }
}
