// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using NSubstitute;

namespace InfiniTests.InfiniFrame.Shared.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class KeyedResultEventTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Add
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Add_NullKey_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act & Assert
        await Assert.That(() => evt.Add(null!, handler: (_, _) => "result")).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_NullHandler_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act & Assert
        await Assert.That(() => evt.Add("key", null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Add_NewKey_IncreasesCount(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act
        evt.Add("a", handler: (_, _) => "result-a");
        evt.Add("b", handler: (_, _) => "result-b");

        // Assert
        await Assert.That(evt.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Add_SameKeyTwice_OverwritesPreviousHandlerAndCountRemainsOne(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => "first");

        // Act
        evt.Add("key", handler: (_, _) => "second");

        // Assert count remains 1 after overwrite
        await Assert.That(evt.Count).IsEqualTo(1);

        // Assert that the second handler is used
        evt.TryInvoke("key", window, 0, out string? result);
        await Assert.That(result).IsEqualTo("second");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Remove
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Remove_NullKey_ThrowsArgumentNullException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act & Assert
        await Assert.That(() => evt.Remove(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Remove_ExistingKey_DecreasesCount(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        evt.Add("key", handler: (_, _) => "r");

        // Act
        evt.Remove("key");

        // Assert
        await Assert.That(evt.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Remove_NonExistentKey_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act & Assert
        await Assert.That(() => evt.Remove("missing")).ThrowsNothing();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ContainsKey
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ContainsKey_AddedKey_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        evt.Add("present", handler: (_, _) => "r");

        // Act & Assert
        await Assert.That(evt.ContainsKey("present")).IsTrue();
    }

    [Test]
    public async Task ContainsKey_MissingKey_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();

        // Act & Assert
        await Assert.That(evt.ContainsKey("absent")).IsFalse();
    }

    [Test]
    public async Task ContainsKey_AfterRemove_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        evt.Add("key", handler: (_, _) => "r");
        evt.Remove("key");

        // Act & Assert
        await Assert.That(evt.ContainsKey("key")).IsFalse();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryInvoke
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryInvoke_MissingKey_ReturnsFalseAndResultIsDefault(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();

        // Act
        bool success = evt.TryInvoke("absent", window, 0, out string? result);

        // Assert
        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task TryInvoke_ExistingKey_ReturnsTrueAndResult(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, v) => $"value={v}");

        // Act
        bool success = evt.TryInvoke("key", window, 7, out string? result);

        // Assert
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo("value=7");
    }

    [Test]
    public async Task TryInvoke_PassesCorrectWindowAndPayloadToHandler(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, string, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        IInfiniFrameWindow? receivedWindow = null;
        string? receivedPayload = null;
        evt.Add("key", handler: (w, p) => {
            receivedWindow = w;
            receivedPayload = p;
            return "ok";
        });

        // Act
        evt.TryInvoke("key", window, "payload", out _);

        // Assert
        await Assert.That(receivedWindow).IsEqualTo(window);
        await Assert.That(receivedPayload).IsEqualTo("payload");
    }

    [Test]
    public async Task TryInvoke_HandlerReturnsNull_ReturnsTrueWithNullResult(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => null!);

        // Act
        bool success = evt.TryInvoke("key", window, 0, out string? result);

        // Assert — a registered handler completed successfully, even when its result is null.
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task TryInvoke_HandlerThrowsRegularException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => throw new InvalidOperationException("boom"));

        // Act & Assert
        await Assert.That(() => { evt.TryInvoke("key", window, 0, out _); }).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task TryInvoke_HandlerThrowsOperationCanceledException_PropagatesException(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => throw new OperationCanceledException());

        // Act & Assert
        await Assert.That(() => {evt.TryInvoke("key", window, 0, out _);}).Throws<OperationCanceledException>();
    }

    [Test]
    public async Task TryInvoke_AfterRemove_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        var window = Substitute.For<IInfiniFrameWindow>();
        evt.Add("key", handler: (_, _) => "r");
        evt.Remove("key");

        // Act
        bool success = evt.TryInvoke("key", window, 0, out string? result);

        // Assert
        await Assert.That(success).IsFalse();
        await Assert.That(result).IsNull();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Count / Handlers
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Count_StartsAtZero(CancellationToken ct = default) {
        // Arrange & Act
        var evt = new KeyedResultEvent<string, int, string>();

        // Assert
        await Assert.That(evt.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Handlers_ContainsAllRegisteredEntries(CancellationToken ct = default) {
        // Arrange
        var evt = new KeyedResultEvent<string, int, string>();
        evt.Add("x", handler: (_, _) => "rx");
        evt.Add("y", handler: (_, _) => "ry");

        // Act & Assert
        await Assert.That(evt.Snapshot.ContainsKey("x")).IsTrue();
        await Assert.That(evt.Snapshot.ContainsKey("y")).IsTrue();
    }
}
