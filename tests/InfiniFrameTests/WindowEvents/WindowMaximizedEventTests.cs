// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowMaximizedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowMaximizedEvent(CancellationToken ct = default) {
        // Arrange
        int maximizedEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterMaximizedHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref maximizedEventCount);
            })
            , ct
        );

        // Act
        windowUtility.Window.SetMaximized(true);
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref maximizedEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(maximizedEventCount).IsEqualTo(1);
    }
}
