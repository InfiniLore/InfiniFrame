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
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterRestoredHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref restoredEventCount);
                })
            , ct
        );

        // Act: maximize first, then restore
        windowUtility.Window.SetMaximized(true);
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref restoredEventCount);
        windowUtility.Window.SetMaximized(false);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref restoredEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }

    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowRestoredFromMinimized(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterRestoredHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref restoredEventCount);
                })
            , ct
        );

        // Act: minimize first, then restore
        windowUtility.Window.SetMinimized(true);
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref restoredEventCount);
        windowUtility.Window.SetMinimized(false);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref restoredEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }
}
