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

        // Assert — event fires synchronously during Build(); no act step needed
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref creatingEventCount), 0, TimeSpan.FromSeconds(5), ct);
        await Assert.That(creatingEventCount).IsEqualTo(1);
    }
}
