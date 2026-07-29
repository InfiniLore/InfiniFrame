// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CloseDuringWebViewInitializationTests {
    [Test]
    [OnlyRunOnWindows]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(20_000)]
    public async Task RepeatedImmediateClose_DoesNotCrashWebView2(CancellationToken ct = default) {
        // Arrange
        const int iterations = 12;

        // Act & Assert
        for (int i = 0; i < iterations; i++) {
            ct.ThrowIfCancellationRequested();

            var windowUtility = InfiniFrameTestWindow.Create(ct);
            IInfiniFrameWindow window = windowUtility.Window;

            windowUtility.Dispose();

            await Assert.That(window.IsClosedOrClosing()).IsTrue();
        }
    }
}
