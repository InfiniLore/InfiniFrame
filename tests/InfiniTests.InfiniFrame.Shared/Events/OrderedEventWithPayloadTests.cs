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
public class OrderedEventWithPayloadTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Add
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();

        // Act & Assert
        await Assert.That(() => orderedEvent.Add(null!)).Throws<ArgumentNullException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Remove
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Remove_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();

        // Act & Assert
        await Assert.That(() => orderedEvent.Remove(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Remove_RegisteredHandler_ReducesSnapshotCount(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        Action<IInfiniFrameWindow, int> handler = (_, _) => { };
        orderedEvent.Add(handler);

        // Act
        orderedEvent.Remove(handler);

        // Assert
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Invoke
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Invoke_SingleHandler_PassesWindowAndPayloadToHandler(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<string>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindow? receivedWindow = null;
        string? receivedPayload = null;

        orderedEvent.Add((w, p) => { receivedWindow = w; receivedPayload = p; });

        // Act
        orderedEvent.Invoke(window, "hello");

        // Assert
        await Assert.That(receivedWindow).IsEqualTo(window);
        await Assert.That(receivedPayload).IsEqualTo("hello");
    }

    [Test]
    public async Task Invoke_MultipleHandlers_AllReceivePayloadInOrder(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        var calls = new List<int>();

        orderedEvent.Add((_, v) => calls.Add(v));
        orderedEvent.Add((_, v) => calls.Add(v + 1));

        // Act
        orderedEvent.Invoke(window, 10);

        // Assert
        await Assert.That(calls).IsEquivalentTo([10, 11]);
    }

    [Test]
    public async Task Invoke_AfterRemove_DoesNotCallRemovedHandler(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        var calls = new List<int>();
        Action<IInfiniFrameWindow, int> removed = (_, _) => calls.Add(99);
        orderedEvent.Add(removed);
        orderedEvent.Add((_, v) => calls.Add(v));
        orderedEvent.Remove(removed);

        // Act
        orderedEvent.Invoke(window, 5);

        // Assert
        await Assert.That(calls).IsEquivalentTo([5]);
    }

    [Test]
    public async Task Invoke_HandlerThrowsException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        orderedEvent.Add((_, _) => throw new InvalidOperationException("boom"));

        // Act & Assert
        await Assert.That(() => orderedEvent.Invoke(window, 0)).Throws<InvalidOperationException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // AddWithServiceResolving
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AddWithServiceResolving_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();

        // Act & Assert
        await Assert.That(() => orderedEvent.AddWithServiceResolving<IDisposable>(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task AddWithServiceResolving_WindowHasNullServiceProvider_ThrowsInvalidOperationException(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        window.ServiceProvider.Returns((IServiceProvider?)null);

        orderedEvent.AddWithServiceResolving<IDisposable>((_, _, _) => { });

        // Act & Assert
        await Assert.That(() => orderedEvent.Invoke(window, 0)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AddWithServiceResolving_WithProvider_ResolvesServiceAndCallsHandler(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        IInfiniFrameWindow window = Substitute.For<IInfiniFrameWindow>();
        IServiceProvider provider = Substitute.For<IServiceProvider>();

        var fakeDisposable = Substitute.For<IDisposable>();
        provider.GetService(typeof(IDisposable)).Returns(fakeDisposable);
        window.ServiceProvider.Returns(provider);

        IDisposable? resolvedService = null;
        orderedEvent.AddWithServiceResolving<IDisposable>((_, _, svc) => resolvedService = svc);

        // Act
        orderedEvent.Invoke(window, 42);

        // Assert
        await Assert.That(resolvedService).IsEqualTo(fakeDisposable);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Snapshot
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Snapshot_IsImmutable_SubsequentAddDoesNotAffectCapturedSnapshot(CancellationToken ct = default) {
        // Arrange
        var orderedEvent = new OrderedEvent<int>();
        orderedEvent.Add((_, _) => { });

        // Act
        ImmutableArray<Action<IInfiniFrameWindow, int>> snapshot = orderedEvent.Snapshot;
        orderedEvent.Add((_, _) => { });

        // Assert
        await Assert.That(snapshot.Length).IsEqualTo(1);
        await Assert.That(orderedEvent.Snapshot.Length).IsEqualTo(2);
    }
}
