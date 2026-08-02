// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CloseSyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(5_000)]
    public async Task Close_Extension_ShouldRequestWindowClose(CancellationToken ct = default) {
        // Arrange
        var windowClosing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.EventsStore.WindowClosingRequested.Add(_ => windowClosing.TrySetResult()),
            ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();

        // Assert
        await windowClosing.Task.WaitAsync(TimeSpan.FromSeconds(3), ct);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(2_000)]
    public async Task Close_Feature_ShouldMarkWindowAsClosing(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Lifecycle.Close();

        // Assert
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}