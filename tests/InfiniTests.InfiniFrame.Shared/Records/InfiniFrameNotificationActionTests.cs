// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNotificationActionTests {

    [Test]
    public async Task Constructor_SetsProperties(CancellationToken ct = default) {
        // Arrange & Act
        var action = new InfiniFrameNotificationAction("Click Me", "action_id");

        // Assert
        await Assert.That(action.Label).IsEqualTo("Click Me");
        await Assert.That(action.Identifier).IsEqualTo("action_id");
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationAction("Label", "id");
        var a2 = new InfiniFrameNotificationAction("Label", "id");

        // Act & Assert
        await Assert.That(a1).IsEqualTo(a2);
    }

    [Test]
    public async Task Equality_DifferentValues_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationAction("Label1", "id");
        var a2 = new InfiniFrameNotificationAction("Label2", "id");

        // Act & Assert
        await Assert.That(a1).IsNotEqualTo(a2);
    }

    [Test]
    public async Task GetHashCode_SameValues_ReturnsSameHash(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationAction("Label", "id");
        var a2 = new InfiniFrameNotificationAction("Label", "id");

        // Act & Assert
        await Assert.That(a1.GetHashCode()).IsEqualTo(a2.GetHashCode());
    }
}
