// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ClearBrowserAutoFillTests {

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert - should not throw on any platform
        Exception? caught = null;
        try {
            window.Features.Browser.ClearBrowserAutoFill();
        }
        catch (Exception ex) {
            caught = ex;
        }

        await Assert.That(caught).IsNull();
    }
}
