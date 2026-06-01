// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowSizeChangedEventTests {
    [Test]
    [Retry(5)]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task TestWindowSizeChangedEvent(CancellationToken ct = default) {
        // Arrange: start at a known size so the second SetSize guarantees a change
        int sizeChangedCount = 0;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder: builder => builder
                .SetSize(800, 600)
                .RegisterSizeChangedHandler((_, _) => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref sizeChangedCount);
                })
            , ct
        );
        int baseline = Volatile.Read(ref sizeChangedCount);

        // Act
        windowUtility.Window.SetSize(400, 300);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref sizeChangedCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(sizeChangedCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
