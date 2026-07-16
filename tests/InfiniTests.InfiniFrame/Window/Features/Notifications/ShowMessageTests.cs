// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ShowMessageTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    public async Task AtWindowStage_DirectAssignment_WhenClosed_ReturnsCancel(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        InfiniFrameDialogResult result = window.Features.Notifications.ShowMessage("title", "body");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionAssignment_WhenClosed_ReturnsCancel(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        InfiniFrameDialogResult result = window.ShowMessage("title", "body");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}
