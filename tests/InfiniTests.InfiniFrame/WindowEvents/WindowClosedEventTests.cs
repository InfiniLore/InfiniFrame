// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task TestWindowClosedEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
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
