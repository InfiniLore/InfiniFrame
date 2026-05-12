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
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterWindowClosedHandler(_ => {
                Interlocked.Increment(ref closedEventCount);
            })
            ,ct
        );

        // Act
        windowUtility.Window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (Volatile.Read(ref closedEventCount) < 1 && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }

        // Assert   
        await Assert.That(closedEventCount).IsEqualTo(1);
    }
}
