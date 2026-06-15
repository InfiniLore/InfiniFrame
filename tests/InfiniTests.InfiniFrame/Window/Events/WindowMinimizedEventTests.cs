// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowMinimizedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMinimized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int minimizedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterMinimizedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref minimizedEventCount);
        }), ct);
        int baseline = Volatile.Read(ref minimizedEventCount);

        // Act
        windowUtility.Window.SetMinimized();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref minimizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(minimizedEventCount).IsEqualTo(baseline + 1);
    }
}
