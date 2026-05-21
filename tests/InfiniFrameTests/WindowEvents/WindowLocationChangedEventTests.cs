// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowLocationChangedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowLocationChangedEvent(CancellationToken ct = default) {
        // Arrange
        int locationChangedCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
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
