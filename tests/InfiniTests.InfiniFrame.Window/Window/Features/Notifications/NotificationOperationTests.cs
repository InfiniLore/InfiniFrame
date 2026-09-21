// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NotificationOperationTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowNotificationAsync_WhenClosed_ReturnsDismissed(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        InfiniFrameNotificationActivation result = await window.ShowNotificationAsync(
            new InfiniFrameNotificationOptions {
                Title = "title",
                Body = "body"
            },
            ct
        );

        // Assert
        await Assert.That(result.Result).IsEqualTo(InfiniFrameNotificationResult.Dismissed);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowNotificationAsync_Cancellation_ReturnsDismissed(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);

        Task<InfiniFrameNotificationActivation> operation = window.ShowNotificationAsync(
            new InfiniFrameNotificationOptions {
                Title = "title",
                Body = "body"
            },
            cancellation.Token
        );
        cancellation.Cancel();

        bool canceled = false;
        try {
            await operation;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) {
            canceled = true;
        }

        // Assert
        await Assert.That(canceled).IsTrue();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}
