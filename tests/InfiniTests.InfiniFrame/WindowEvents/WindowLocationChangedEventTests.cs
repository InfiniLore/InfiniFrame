// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowLocationChangedEventTests {
    [Test, Retry(5), SkipOnMacOs, SkipOnLinux("Location transitions are desktop-state dependent under WSLg/local Linux runs"), NotInParallelInfiniTests]
    public async Task TestWindowLocationChangedEvent(CancellationToken ct = default) {
        // Arrange
        int locationChangedCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterLocationChangedHandler((_, _) => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref locationChangedCount);
                })
            , ct
        );

        // Act: move to a known position first to establish a stable baseline,
        // then record the count and move to a different position
        windowUtility.Window.SetLocation(50, 50);
        int baseline = Volatile.Read(ref locationChangedCount);
        windowUtility.Window.SetLocation(150, 150);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref locationChangedCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(locationChangedCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
