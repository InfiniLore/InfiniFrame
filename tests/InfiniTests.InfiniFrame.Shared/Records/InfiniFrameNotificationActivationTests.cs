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
    [Arguments(InfiniFrameNotificationResult.ActionClicked, "action_id")]
    [Arguments(InfiniFrameNotificationResult.BodyClicked, null)]
    [Arguments(InfiniFrameNotificationResult.Dismissed, null)]
    [Arguments(InfiniFrameNotificationResult.TimedOut, null)]
    [Arguments(InfiniFrameNotificationResult.Failed, null)]
    public async Task Constructor_SetsResultAndActionIdentifier(InfiniFrameNotificationResult result, string? actionId, CancellationToken ct = default) {
        // Arrange & Act
        var activation = new InfiniFrameNotificationActivation(result, actionId);

        // Assert
        await Assert.That(activation.Result).IsEqualTo(result);
        await Assert.That(activation.ActionIdentifier).IsEqualTo(actionId);
    }

    [Test]
    [Arguments(InfiniFrameNotificationResult.ActionClicked)]
    [Arguments(InfiniFrameNotificationResult.BodyClicked)]
    [Arguments(InfiniFrameNotificationResult.Dismissed)]
    [Arguments(InfiniFrameNotificationResult.TimedOut)]
    [Arguments(InfiniFrameNotificationResult.Failed)]
    public async Task Equality_SameValues_ReturnsTrue(InfiniFrameNotificationResult result, CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationActivation(result);
        var a2 = new InfiniFrameNotificationActivation(result);

        // Assert
        await Assert.That(a1).IsEqualTo(a2);
    }

    [Test]
    public async Task Equality_DifferentResult_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var a1 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed);
        var a2 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.TimedOut);

        // Assert
        await Assert.That(a1).IsNotEqualTo(a2);
    }

    [Test]
    public async Task AllResultValues_AreDefined(CancellationToken ct = default) {
        // Arrange
        InfiniFrameNotificationResult[] values = Enum.GetValues<InfiniFrameNotificationResult>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(5);
    }
}
