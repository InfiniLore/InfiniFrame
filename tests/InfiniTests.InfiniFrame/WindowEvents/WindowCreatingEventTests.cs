// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatingEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task TestWindowCreatingEvent(CancellationToken ct = default) {
        // Arrange
        int creatingEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterWindowCreatingHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref creatingEventCount);
                })
            , ct
        );

        // Assert: event fires synchronously during Build(); no act step needed
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref creatingEventCount), 0, TimeSpan.FromSeconds(5), ct);
        await Assert.That(creatingEventCount).IsEqualTo(1);
    }
}
