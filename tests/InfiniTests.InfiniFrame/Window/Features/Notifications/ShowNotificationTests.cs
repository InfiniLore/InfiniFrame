// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ShowNotificationTests {
    [Test]
    [SkipOnMacOs]
    public async Task AtWindowStage_DirectAssignment_WhenClosed_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        window.Features.Notifications.ShowNotification("title", "body");

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionAssignment_WhenClosed_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        IInfiniFrameWindow returnedWindow = window.ShowNotification("title", "body");

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}
