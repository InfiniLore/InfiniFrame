// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusInEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [SkipOnLinux("Focus transitions are desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task TestWindowFocusInEvent(CancellationToken ct = default) {
        // Arrange
        int focusInEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterFocusInHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref focusInEventCount);
                })
            , ct
        );

        // Minimize first to guarantee the window is not focused, so that SetFocused()
        // below produces a clean FocusIn transition (WM_ACTIVATE with WA_ACTIVE).
        // Without this, the window may already be active from ShowWindow during Build(),
        // and Win32 will not re-send WM_ACTIVATE to an already-active window.
        windowUtility.Window.SetMinimized(true);
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref focusInEventCount);

        // Act: restores the window and brings it to the foreground → FocusIn
        windowUtility.Window.SetFocused();

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref focusInEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(focusInEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
