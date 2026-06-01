// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
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
        int baseline = Volatile.Read(ref maximizedEventCount);
        
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterMaximizedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref maximizedEventCount);
                })
            , ct
        );

        // Act
        windowUtility.Window.SetMaximized(true);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref maximizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(maximizedEventCount).IsEqualTo(baseline + 1);
    }
}
