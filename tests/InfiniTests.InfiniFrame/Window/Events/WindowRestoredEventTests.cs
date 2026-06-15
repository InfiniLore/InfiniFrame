// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRestoredEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_RestoreFromMaximized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterRestoredHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref restoredEventCount);
        }), ct);

        windowUtility.Window.SetMaximized();
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref restoredEventCount);

        // Act
        windowUtility.Window.SetMaximized(false);

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref restoredEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }

    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_RestoreFromMinimized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterRestoredHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref restoredEventCount);
        }), ct);

        windowUtility.Window.SetMinimized();
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref restoredEventCount);

        // Act
        windowUtility.Window.SetMinimized(false);

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref restoredEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }
}
