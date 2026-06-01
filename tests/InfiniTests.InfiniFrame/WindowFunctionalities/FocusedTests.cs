// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FocusedTests {
    [Test, DisplayName($"{nameof(FocusedTests)}.{nameof(Window)}"), SkipOnMacOs(SkipUtility.MacOsMainThreadIssue), SkipOnLinux("Given that the window is virtualized, this test is not applicable."), NotInParallelInfiniTests, Retry(5)]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        // await Task.Delay(10000); // Uncomment this if you want to manually check this, otherwise it will always be focused
        window.SetFocused();

        // Assert
        await Assert.That(window.Focused).IsTrue();
    }
}
