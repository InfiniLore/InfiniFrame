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
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterLocationChangedHandler((_, _) => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref locationChangedCount);
            })
            , ct
        );

        // Act
        windowUtility.Window.MoveWithinCurrentMonitorArea(100, 100);
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref locationChangedCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(locationChangedCount).IsGreaterThanOrEqualTo(1);
    }
}
