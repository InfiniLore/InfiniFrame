// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNotificationActivationTests {

    [Test]
    public async Task Constructor_SetsResultAndActionIdentifier(CancellationToken ct = default) {
        // Arrange & Act
        var activation = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.ActionClicked, "action_id");

        // Assert
        await Assert.That(activation.Result).IsEqualTo(InfiniFrameNotificationResult.ActionClicked);
        await Assert.That(activation.ActionIdentifier).IsEqualTo("action_id");
    }

    [Test]
    public async Task Constructor_DefaultActionIdentifier_IsNull(CancellationToken ct = default) {
        // Arrange & Act
        var activation = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.BodyClicked);

        // Assert
        await Assert.That(activation.Result).IsEqualTo(InfiniFrameNotificationResult.BodyClicked);
        await Assert.That(activation.ActionIdentifier).IsNull();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed);
        var a2 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed);

        // Act & Assert
        await Assert.That(a1).IsEqualTo(a2);
    }

    [Test]
    public async Task Equality_DifferentResult_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed);
        var a2 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.TimedOut);

        // Act & Assert
        await Assert.That(a1).IsNotEqualTo(a2);
    }

    [Test]
    public async Task AllResultValues_AreDefined(CancellationToken ct = default) {
        InfiniFrameNotificationResult[] values = Enum.GetValues<InfiniFrameNotificationResult>();
        int count = values.Length;
        await Assert.That(count).IsEqualTo(5);
    }
}
