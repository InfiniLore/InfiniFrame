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
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    // [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task TestWindowClosedEvent(CancellationToken ct) {
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

        // Assert   
        await Assert.That(closedEventCount).IsEqualTo(1);
    }
}
