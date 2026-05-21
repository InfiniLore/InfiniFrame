// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowMinimizedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowMinimizedEvent(CancellationToken ct = default) {
        // Arrange
        int minimizedEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterMinimizedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref minimizedEventCount);
                })
            , ct
        );
        int baseline = Volatile.Read(ref minimizedEventCount);

        // Act
        windowUtility.Window.SetMinimized(true);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref minimizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(minimizedEventCount).IsEqualTo(baseline + 1);
    }
}
