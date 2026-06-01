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
    [SkipUtility.SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowMaximizedEvent(CancellationToken ct = default) {
        // Arrange
        int maximizedEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterMaximizedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref maximizedEventCount);
                })
            , ct
        );
        int baseline = Volatile.Read(ref maximizedEventCount);

        // Act
        windowUtility.Window.SetMaximized(true);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref maximizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(maximizedEventCount).IsEqualTo(baseline + 1);
    }
}
