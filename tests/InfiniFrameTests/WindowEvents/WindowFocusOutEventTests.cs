// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusOutEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowFocusOutEvent(CancellationToken ct = default) {
        // Arrange
        int focusOutEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterFocusOutHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref focusOutEventCount);
            })
            , ct
        );

        // Act — minimize causes the window to lose focus
        windowUtility.Window.SetFocused();
        await Task.Delay(100, ct);
        windowUtility.Window.SetMinimized(true);

        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref focusOutEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert
        await Assert.That(focusOutEventCount).IsGreaterThanOrEqualTo(1);
    }
}
