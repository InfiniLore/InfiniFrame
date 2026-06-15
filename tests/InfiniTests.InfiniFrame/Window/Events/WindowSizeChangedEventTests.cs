// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowSizeChangedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetSize_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int sizeChangedCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
            .SetSize(800, 600)
            .RegisterSizeChangedHandler((_, _) => {
                // ReSharper disable once AccessToModifiedClosure
                Interlocked.Increment(ref sizeChangedCount);
            }), ct);
        int baseline = Volatile.Read(ref sizeChangedCount);

        // Act
        windowUtility.Window.SetSize(400, 300);

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref sizeChangedCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(sizeChangedCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
