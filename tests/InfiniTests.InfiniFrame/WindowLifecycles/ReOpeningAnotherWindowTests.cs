// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniTests.InfiniFrame.WindowLifecycles;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ReOpeningAnotherWindowTests {
    [Test]
    [NotInParallelInfiniTests]
    [Timeout(1000000000)]
    public async Task CreateAnotherWindowAfterClosingOthers(CancellationToken ct = default) {
        // Arrange
        var window1Utility = InfiniFrameTestWindow.Create(ct);
        var window2Utility = InfiniFrameTestWindow.Create(ct);

        try {
            await Task.Run(() => window1Utility.Window.Close(), ct);
            window1Utility.Window.WaitForClose();

            // Act
            await Task.Run(() => window2Utility.Window.Close(), ct);
            window1Utility.Window.WaitForClose();

            // Assert
            await Assert.That(window1Utility.Window.IsClosed).IsTrue();
            await Assert.That(window2Utility.Window.IsClosed).IsTrue();
        }
        finally {
            window1Utility.Dispose();
            window2Utility.Dispose();
        }
    }
}
