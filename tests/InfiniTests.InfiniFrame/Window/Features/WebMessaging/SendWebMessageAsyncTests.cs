// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniTests.Substitutes;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SendWebMessageAsyncTests {
    [Test]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var windowUtility = new RecordingInfiniFrameWindowSubstitute();
        IInfiniFrameWindow window = windowUtility.Window;
        const string message = "test-async-message-direct";

        // Act
        await window.Features.WebMessaging.SendWebMessageAsync(message, ct);
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
        const string message = "test-async-message-extension";

        // Act
        await window.SendWebMessageAsync(message, ct);
        IReadOnlyList<string> sentMessages = windowUtility.GetSentMessagesSnapshot();

        // Assert
        await Assert.That(sentMessages.Count).IsEqualTo(1);
        await Assert.That(sentMessages[0]).IsEqualTo(message);
    }

    [Test]
    public async Task AtWindowStage_CanceledToken_ReturnsCanceled(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act + Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () => {
            await window.Features.WebMessaging.SendWebMessageAsync("noop", cts.Token);
        });
        await Assert.ThrowsAsync<TaskCanceledException>(async () => {
            await window.SendWebMessageAsync("noop", cts.Token);
        });
    }
}
