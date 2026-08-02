// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests.Substitutes;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SendWebMessageTests {
    [Test]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var windowUtility = new RecordingInfiniFrameWindowSubstitute();
        IInfiniFrameWindow window = windowUtility.Window;
        const string message = "test-message-direct";

        // Act
        window.Features.WebMessaging.SendWebMessage(message);
        IReadOnlyList<string> sentMessages = windowUtility.GetSentMessagesSnapshot();

        // Assert
        await Assert.That(sentMessages.Count).IsEqualTo(1);
        await Assert.That(sentMessages[0]).IsEqualTo(message);
    }

    [Test]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        var windowUtility = new RecordingInfiniFrameWindowSubstitute();
        IInfiniFrameWindow window = windowUtility.Window;
        const string message = "test-message-extension";

        // Act
        window.SendWebMessage(message);
        IReadOnlyList<string> sentMessages = windowUtility.GetSentMessagesSnapshot();

        // Assert
        await Assert.That(sentMessages.Count).IsEqualTo(1);
        await Assert.That(sentMessages[0]).IsEqualTo(message);
    }
}