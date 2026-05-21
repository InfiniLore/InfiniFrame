// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowClosedEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterWindowClosedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref closedEventCount);
                })
            , ct
        );
        int baseline = Volatile.Read(ref closedEventCount);

        // Act
        windowUtility.Window.Close();

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref closedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closedEventCount).IsEqualTo(baseline + 1);
    }
}
