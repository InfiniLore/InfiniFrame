// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRestoredEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowRestoredFromMaximized(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterRestoredHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref restoredEventCount);
            })
            , ct
        );

        // Act — maximize first, then restore
        windowUtility.Window.SetMaximized(true);
        await Task.Delay(100, ct);
        windowUtility.Window.SetMaximized(false);

        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref restoredEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(restoredEventCount).IsEqualTo(1);
    }

    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowRestoredFromMinimized(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterRestoredHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref restoredEventCount);
            })
            , ct
        );

        // Act — minimize first, then restore
        windowUtility.Window.SetMinimized(true);
        await Task.Delay(100, ct);
        windowUtility.Window.SetMinimized(false);

        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref restoredEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(restoredEventCount).IsEqualTo(1);
    }
}
