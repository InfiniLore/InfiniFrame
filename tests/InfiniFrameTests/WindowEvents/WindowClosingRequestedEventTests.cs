// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosingRequestedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowClosingRequestedEvent(CancellationToken ct = default) {
        // Arrange
        int closingRequestedEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .RegisterWindowClosingRequestedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref closingRequestedEventCount);
                })
            , ct
        );
        int baseline = Volatile.Read(ref closingRequestedEventCount);

        // Act
        windowUtility.Window.Close();

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref closingRequestedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closingRequestedEventCount).IsEqualTo(baseline + 1);
    }
}
