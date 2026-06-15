// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusInEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [SkipOnLinux("Focus transitions are desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetFocused_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int focusInEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterFocusInHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref focusInEventCount);
        }), ct);

        windowUtility.Window.SetMinimized();
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref focusInEventCount);

        // Act
        windowUtility.Window.SetFocused();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref focusInEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(focusInEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
