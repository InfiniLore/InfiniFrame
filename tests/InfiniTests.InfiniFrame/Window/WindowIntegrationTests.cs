// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class WindowIntegrationTests {
    [Test]
    [SkipOnLinux]
    public async Task FullscreenAndResize_Interaction_RemainsDeterministic(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(800, 520);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetFullScreen();
        await WaitForConditionAsync(() => window.Features.State.IsFullScreen, TimeSpan.FromSeconds(5), ct);

        window.Features.State.SetFullScreen(false);
        await WaitForConditionAsync(() => !window.Features.State.IsFullScreen, TimeSpan.FromSeconds(5), ct);

        int widthBefore = window.Features.Size.Width;
        int heightBefore = window.Features.Size.Height;
        window.Features.Size.Resize(120, 80, ResizeOrigin.BottomRight);

        // Assert
        int widthAfter = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Width, widthBefore, TimeSpan.FromSeconds(5), ct);
        int heightAfter = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Height, heightBefore, TimeSpan.FromSeconds(5), ct);
        await Assert.That(widthAfter).IsGreaterThan(widthBefore);
        await Assert.That(heightAfter).IsGreaterThan(heightBefore);
        await Assert.That(window.Features.State.IsFullScreen).IsFalse();
    }

    [Test]
    [SkipOnLinux("Maximize verification is unsupported on Linux")]
    public async Task MaximizeAndMove_Interaction_RemainsDeterministic(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetMaximized();
        await WaitForConditionAsync(() => window.Features.State.IsMaximized, TimeSpan.FromSeconds(5), ct);

        window.Features.State.SetMaximized(false);
        await WaitForConditionAsync(() => !window.Features.State.IsMaximized, TimeSpan.FromSeconds(5), ct);

        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        int targetLeft = originalLeft + 40;
        int targetTop = originalTop + 40;

        window.Features.Position.SetLocation(targetLeft, targetTop);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int updatedTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsEqualTo(targetLeft);
        await Assert.That(updatedTop).IsEqualTo(targetTop);
        await Assert.That(window.Features.State.IsMaximized).IsFalse();
    }

    [Test]
    [SkipOnLinux]
    public async Task ConcurrentResizeMoveStateCalls_ResultInDeterministicFinalState(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(720, 440);
            builder.Features.Position.SetLocation(120, 140);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        int initialWidth = window.Features.Size.Width;
        int initialHeight = window.Features.Size.Height;
        int initialLeft = window.Features.Position.Left;
        int initialTop = window.Features.Position.Top;
        int targetWidth = initialWidth + 100;
        int targetHeight = initialHeight + 60;
        int targetLeft = initialLeft + 60;
        int targetTop = initialTop + 60;

        // Act
        Task resizeTask = Task.Run(() => window.Features.Size.Resize(100, 60, ResizeOrigin.BottomRight), ct);
        Task moveTask = Task.Run(() => window.Features.Position.SetLocation(targetLeft, targetTop), ct);
        Task stateTask = Task.Run(() => window.Features.State.SetFullScreen(false), ct);
        await Task.WhenAll(resizeTask, moveTask, stateTask);

        // Reconcile to an explicit final state after concurrent operations.
        window.Features.State.SetFullScreen(false);
        window.Features.Position.SetLocation(targetLeft, targetTop);
        window.Features.Size.SetSize(targetWidth, targetHeight);

        await WaitForConditionAsync(() => window.Features.Size.Width == targetWidth, TimeSpan.FromSeconds(5), ct);
        await WaitForConditionAsync(() => window.Features.Size.Height == targetHeight, TimeSpan.FromSeconds(5), ct);
        await WaitForConditionAsync(() => window.Features.Position.Left == targetLeft, TimeSpan.FromSeconds(5), ct);
        await WaitForConditionAsync(() => window.Features.Position.Top == targetTop, TimeSpan.FromSeconds(5), ct);

        // Assert
        int finalWidth = window.Features.Size.Width;
        int finalHeight = window.Features.Size.Height;
        int finalLeft = window.Features.Position.Left;
        int finalTop = window.Features.Position.Top;
        await Assert.That(finalWidth).IsGreaterThan(initialWidth);
        await Assert.That(finalHeight).IsGreaterThan(initialHeight);
        await Assert.That(finalLeft).IsEqualTo(targetLeft);
        await Assert.That(finalTop).IsEqualTo(targetTop);
        await Assert.That(window.Features.State.IsFullScreen).IsFalse();
    }

    private static async Task WaitForConditionAsync(
        Func<bool> condition,
        TimeSpan timeout,
        CancellationToken ct
    ) {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (!condition()) {
            if (DateTime.UtcNow >= deadline) {
                throw new TimeoutException($"Condition was not met within {timeout}.");
            }

            await Task.Delay(50, ct);
        }
    }
}
