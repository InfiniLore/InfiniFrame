// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowLocationChangedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [SkipOnLinux("Location transitions are desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetLocation_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int locationChangedCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterLocationChangedHandler((_, _) => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref locationChangedCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);

        windowUtility.Window.SetLocation(50, 50);
        baseline = Volatile.Read(ref locationChangedCount);

        // Act
        windowUtility.Window.SetLocation(150, 150);

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(locationChangedCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
