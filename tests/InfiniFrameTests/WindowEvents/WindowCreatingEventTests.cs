// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatingEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowCreatingEvent(CancellationToken ct = default) {
        // Arrange
        int creatingEventCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder
            .RegisterWindowCreatingHandler(_ => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref creatingEventCount);
            })
            , ct
        );

        // Assert
        await Assert.That(creatingEventCount).IsEqualTo(1);
    }
}
