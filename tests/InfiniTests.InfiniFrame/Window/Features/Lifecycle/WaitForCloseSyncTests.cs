// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WaitForCloseSyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task WaitForClose_Extension_ShouldCompleteWhenWindowCloses(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task waitTask = Task.Run(window.WaitForClose, ct);
        await Task.Delay(200, ct);
        window.Close();

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task WaitForClose_Feature_ShouldCompleteWhenWindowCloses(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task waitTask = Task.Run(window.Features.Lifecycle.WaitForClose, ct);
        await Task.Delay(200, ct);
        window.Features.Lifecycle.Close();

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}
