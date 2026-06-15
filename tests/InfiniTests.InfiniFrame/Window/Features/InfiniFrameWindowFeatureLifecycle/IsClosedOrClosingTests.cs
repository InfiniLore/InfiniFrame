// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureLifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IsClosedOrClosingTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(6_000)]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool beforeClose = window.IsClosedOrClosing();
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(beforeClose).IsFalse();
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [DefaultInfiniTestsTimeout(6_000)]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool beforeClose = window.Features.Lifecycle.IsClosedOrClosing();
        window.Features.Lifecycle.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.Features.Lifecycle.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(beforeClose).IsFalse();
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}
