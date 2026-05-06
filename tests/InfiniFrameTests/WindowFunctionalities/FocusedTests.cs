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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [Retry(5)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        
        // Act
        // await Task.Delay(10000); // Uncomment this if you want to manually check this, otherwise it will always be focused
        window.SetFocused();

        // Assert
        const int maxAttempts = 20;
        for (int i = 0; i < maxAttempts && !window.Focused; i++) {
            await Task.Delay(50, ct);
        }

        if (!window.Focused) {
            Skip.Test("Unable to acquire window focus in this Windows session.");
            return;
        }

        await Assert.That(window.Focused).IsTrue();
    }
}
