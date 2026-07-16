// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WaitForCloseTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task waitTask = Task.Run(() => window.WaitForClose(), ct);
        await Task.Delay(200, ct);
        window.Close();

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task waitTask = Task.Run(async () => {
            await window.Features.Lifecycle.WaitForCloseAsync(ct);
        }, ct);
        await Task.Delay(200, ct);
        await window.Features.Lifecycle.CloseAsync(ct);

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}
