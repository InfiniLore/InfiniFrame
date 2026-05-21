// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowCreatedEvent(CancellationToken ct = default) {
        // Arrange
        int createdEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterWindowCreatedHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref createdEventCount);
            })
            , ct
        );

        // Assert
        await Assert.That(createdEventCount).IsEqualTo(1);
    }
}
