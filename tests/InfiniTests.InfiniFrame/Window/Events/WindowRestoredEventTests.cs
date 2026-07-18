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
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_RestoreFromMaximized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterRestoredHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref restoredEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);

        windowUtility.Window.SetMaximized();
        await Task.Delay(100, ct);
        baseline = Volatile.Read(ref restoredEventCount);

        // Act
        windowUtility.Window.SetMaximized(false);

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_RestoreFromMinimized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int restoredEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterRestoredHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref restoredEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);

        windowUtility.Window.SetMinimized();
        await Task.Delay(100, ct);
        baseline = Volatile.Read(ref restoredEventCount);

        // Act
        windowUtility.Window.SetMinimized(false);

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(restoredEventCount).IsEqualTo(baseline + 1);
    }
}
