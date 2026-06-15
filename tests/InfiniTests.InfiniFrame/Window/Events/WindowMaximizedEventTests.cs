// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowMaximizedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMaximized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int maximizedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterMaximizedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref maximizedEventCount);
        }), ct);
        int baseline = Volatile.Read(ref maximizedEventCount);

        // Act
        windowUtility.Window.SetMaximized();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref maximizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(maximizedEventCount).IsEqualTo(baseline + 1);
    }
}
