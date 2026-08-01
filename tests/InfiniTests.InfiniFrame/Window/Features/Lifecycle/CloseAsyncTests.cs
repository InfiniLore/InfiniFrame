// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CloseAsyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task CloseAsync_Extension_ShouldRequestWindowClose(CancellationToken ct = default) {
        // Arrange
        var windowClosing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.EventsStore.WindowClosingRequested.Add(_ => windowClosing.TrySetResult()),
            ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        await window.CloseAsync(ct);

        // Assert
        await windowClosing.Task.WaitAsync(TimeSpan.FromSeconds(3), ct);
        if (!window.IsClosedOrClosing())
            throw new InvalidOperationException("CloseAsync completed before the native window entered a closed state.");
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task CloseAsync_Feature_ShouldMarkWindowAsClosing(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        await window.Features.Lifecycle.CloseAsync(ct);

        // Assert
        if (!window.Features.Lifecycle.IsClosedOrClosing())
            throw new InvalidOperationException("CloseAsync completed before the native window entered a closed state.");
    }
}
