// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FocusedTests {
    [Test]
    [DisplayName($"{nameof(FocusedTests)}.{nameof(Window)}")]   
    [SkipUtility.SkipOnMacOs(SkipUtility.MacOsMainThreadIssue)]
    [SkipUtility.SkipOnLinux("Given that the window is virtualized, this test is not applicable.")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtlitity.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act
        // await Task.Delay(10000); // Uncomment this if you want to manually check this, otherwise it will always be focused
        window.SetFocused();

        // Assert
        await Assert.That(window.Focused).IsTrue();
    }
}
