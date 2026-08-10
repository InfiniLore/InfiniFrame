// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;
using System.Collections.Immutable;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OrderedResultEventTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Add
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();

        // Act & Assert
        await Assert.That(() => evt.Add(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_SingleHandler_SnapshotContainsOneEntry(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();

        // Act
        evt.Add((_, _) => "result");

        // Assert
        await Assert.That(evt.Snapshot.Length).IsEqualTo(1);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Remove
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Remove_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();

        // Act & Assert
        await Assert.That(() => evt.Remove(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Remove_RegisteredHandler_ReducesSnapshotCount(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        Func<IInfiniFrameWindow, int, string> handler = (_, _) => "r";
        evt.Add(handler);

        // Act
        evt.Remove(handler);

        // Assert
        await Assert.That(evt.Snapshot.Length).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Invoke
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Invoke_NoHandlers_ReturnsEmptyArray(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();

        // Act
        string?[] result = evt.Invoke(window, 0);

        // Assert
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task Invoke_SingleHandler_ReturnsResultInArray(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add((_, v) => $"value={v}");

        // Act
        string?[] result = evt.Invoke(window, 42);

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        await Assert.That(result[0]).IsEqualTo("value=42");
    }

    [Test]
    public async Task Invoke_MultipleHandlers_ReturnsAllResultsInRegistrationOrder(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add((_, _) => "first");
        evt.Add((_, _) => "second");
        evt.Add((_, _) => "third");

        // Act
        string?[] result = evt.Invoke(window, 0);

        // Assert
        await Assert.That(result[0]).IsEqualTo("first");
        await Assert.That(result[1]).IsEqualTo("second");
        await Assert.That(result[2]).IsEqualTo("third");
    }

    [Test]
    public async Task Invoke_HandlerThrowsRegularException_PropagatesAndStopsDispatch(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add((_, _) => "before");
        evt.Add((_, _) => throw new InvalidOperationException("boom"));
        evt.Add((_, _) => "after");

        // Act & Assert
        await Assert.That(() => evt.Invoke(window, 0)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Invoke_HandlerThrowsOperationCanceledException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add((_, _) => throw new OperationCanceledException());

        // Act & Assert
        await Assert.That(() => evt.Invoke(window, 0)).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task Invoke_AfterRemove_DoesNotIncludeRemovedHandlerResult(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        Func<IInfiniFrameWindow, int, string> removed = (_, _) => "removed";
        evt.Add(removed);
        evt.Add((_, _) => "kept");
        evt.Remove(removed);

        // Act
        string?[] result = evt.Invoke(window, 0);

        // Assert
        await Assert.That(result.Length).IsEqualTo(1);
        await Assert.That(result[0]).IsEqualTo("kept");
    }

    [Test]
    public async Task Invoke_PassesWindowAndPayloadToEachHandler(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<string, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindow? receivedWindow = null;
        string? receivedPayload = null;
        evt.Add((w, p) => {
            receivedWindow = w;
            receivedPayload = p;
            return "ok";
        });

        // Act
        evt.Invoke(window, "payload");

        // Assert
        await Assert.That(receivedWindow).IsEqualTo(window);
        await Assert.That(receivedPayload).IsEqualTo("payload");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Snapshot
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Snapshot_StartsEmpty(CancellationToken ct = default) {
        // Arrange & Act
        var evt = new OrderedResultEvent<int, string>();

        // Assert
        await Assert.That(evt.Snapshot.ToArray()).IsEmpty();
    }

    [Test]
    public async Task Snapshot_IsImmutable_SubsequentAddDoesNotAffectCapturedSnapshot(CancellationToken ct = default) {
        // Arrange
        var evt = new OrderedResultEvent<int, string>();
        evt.Add((_, _) => "a");

        // Act
        ImmutableArray<Func<IInfiniFrameWindow, int, string>> snapshot = evt.Snapshot;
        evt.Add((_, _) => "b");

        // Assert
        await Assert.That(snapshot.Length).IsEqualTo(1);
        await Assert.That(evt.Snapshot.Length).IsEqualTo(2);
    }
}
