// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class KeyedEventTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Add
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_NullKey_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act & Assert
        await Assert.That(() => evt.Add(null!, handler: (_, _) => { })).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act & Assert
        await Assert.That(() => evt.Add("key", null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_NewKey_IncreasesCount(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act
        evt.Add("a", handler: (_, _) => { });
        evt.Add("b", handler: (_, _) => { });

        // Assert
        await Assert.That(evt.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Add_SameKeyTwice_OverwritesPreviousHandlerAndCountRemainsOne(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var calls = new List<string>();
        evt.Add("key", handler: (_, _) => calls.Add("first"));

        // Act — second add with same key replaces the first handler
        evt.Add("key", handler: (_, _) => calls.Add("second"));

        // Assert count
        await Assert.That(evt.Count).IsEqualTo(1);

        // Assert that Invoke calls the second handler, not the first
        evt.TryInvoke("key", Substitute.For<IInfiniFrameWindow>(), 0);
        await Assert.That(calls).IsEquivalentTo(["second"]);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Remove
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Remove_NullKey_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act & Assert
        await Assert.That(() => evt.Remove(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Remove_ExistingKey_DecreasesCount(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        evt.Add("key", handler: (_, _) => { });

        // Act
        evt.Remove("key");

        // Assert
        await Assert.That(evt.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Remove_NonExistentKey_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act & Assert
        await Assert.That(() => evt.Remove("missing")).ThrowsNothing();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ContainsKey
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ContainsKey_AddedKey_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        evt.Add("present", handler: (_, _) => { });

        // Act & Assert
        await Assert.That(evt.ContainsKey("present")).IsTrue();
    }

    [Test]
    public async Task ContainsKey_MissingKey_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();

        // Act & Assert
        await Assert.That(evt.ContainsKey("absent")).IsFalse();
    }

    [Test]
    public async Task ContainsKey_AfterRemove_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        evt.Add("key", handler: (_, _) => { });
        evt.Remove("key");

        // Act & Assert
        await Assert.That(evt.ContainsKey("key")).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryInvoke
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryInvoke_MissingKey_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();

        // Act
        bool result = evt.TryInvoke("absent", window, 0);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryInvoke_ExistingKey_InvokesHandlerAndReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        var calls = new List<int>();
        evt.Add("key", handler: (_, v) => calls.Add(v));

        // Act
        bool result = evt.TryInvoke("key", window, 99);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(calls).IsEquivalentTo([99]);
    }

    [Test]
    public async Task TryInvoke_PassesCorrectWindowToHandler(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindow? received = null;
        evt.Add("key", handler: (w, _) => received = w);

        // Act
        evt.TryInvoke("key", window, 0);

        // Assert
        await Assert.That(received).IsEqualTo(window);
    }

    [Test]
    public async Task TryInvoke_HandlerThrowsRegularException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => throw new InvalidOperationException("boom"));

        // Act & Assert
        await Assert.That(() => evt.TryInvoke("key", window, 0)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task TryInvoke_HandlerThrowsOperationCanceledException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => throw new OperationCanceledException());

        // Act & Assert
        await Assert.That(() => evt.TryInvoke("key", window, 0)).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task TryInvoke_AfterRemove_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => { });
        evt.Remove("key");

        // Act
        bool result = evt.TryInvoke("key", window, 0);

        // Assert
        await Assert.That(result).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Snapshot
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Snapshot_ContainsAllRegisteredHandlers(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedEvent<string, int>();
        evt.Add("a", handler: (_, _) => { });
        evt.Add("b", handler: (_, _) => { });

        // Act
        List<KeyValuePair<string, Action<IInfiniFrameWindow, int>>> snapshot = evt.Snapshot.ToList();

        // Assert
        await Assert.That(snapshot.Count).IsEqualTo(2);
        await Assert.That(snapshot.Any(kvp => kvp.Key == "a")).IsTrue();
        await Assert.That(snapshot.Any(kvp => kvp.Key == "b")).IsTrue();
    }

    [Test]
    public async Task Count_StartsAtZero(CancellationToken ct = default) {
        // Arrange & Act
        var evt = new KeyedEvent<string, int>();

        // Assert
        await Assert.That(evt.Count).IsEqualTo(0);
    }
}