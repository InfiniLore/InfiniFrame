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
        int baseline = Volatile.Read(ref focusInEventCount);

        // Act
        windowUtility.Window.SetFocused();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref focusInEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(focusInEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
