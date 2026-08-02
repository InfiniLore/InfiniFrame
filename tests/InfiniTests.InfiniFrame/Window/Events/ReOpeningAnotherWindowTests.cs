// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ReOpeningAnotherWindowTests {
    [Test]
    [SkipOnWindowsArm]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_CloseMultipleWindows_DoesNotPreventSubsequentWindowCreation(CancellationToken ct = default) {
        // Arrange
        var window1Utility = InfiniFrameTestWindow.Create(ct);
        var window2Utility = InfiniFrameTestWindow.Create(ct);

        try {
            // Act
            await Task.Run(window1Utility.Window.Close, ct);
            window1Utility.Window.WaitForClose();
            await Task.Run(window2Utility.Window.Close, ct);
            window2Utility.Window.WaitForClose();

            // Assert
            await Assert.That(window1Utility.Window.IsClosedOrClosing()).IsTrue();
            await Assert.That(window2Utility.Window.IsClosedOrClosing()).IsTrue();
        }
        finally {
            window1Utility.Dispose();
            window2Utility.Dispose();
        }
    }
}