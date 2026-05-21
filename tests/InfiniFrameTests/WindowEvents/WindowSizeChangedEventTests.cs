// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowSizeChangedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowSizeChangedEvent(CancellationToken ct = default) {
        // Arrange
        int sizeChangedCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterSizeChangedHandler((_, _) => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref sizeChangedCount);
            })
            , ct
        );

        // Act
        windowUtility.Window.SetSize(640, 480);
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref sizeChangedCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(sizeChangedCount).IsGreaterThanOrEqualTo(1);
    }
}
