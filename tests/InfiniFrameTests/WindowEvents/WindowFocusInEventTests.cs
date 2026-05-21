// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusInEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowFocusInEvent(CancellationToken ct = default) {
        // Arrange
        int focusInEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterFocusInHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref focusInEventCount);
            })
            , ct
        );

        // Act
        windowUtility.Window.SetFocused();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref focusInEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(focusInEventCount).IsGreaterThanOrEqualTo(1);
    }
}
