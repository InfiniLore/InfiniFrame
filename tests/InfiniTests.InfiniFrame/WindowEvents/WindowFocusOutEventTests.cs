// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusOutEventTests {
    [Test, Retry(5), SkipOnMacOs, SkipOnWindowsArm("WM_ACTIVATE WA_INACTIVE is not reliably delivered on headless ARM64 CI runners"), SkipOnLinux("Focus transitions are desktop-state dependent under WSLg/local Linux runs"), NotInParallelInfiniTests]
    public async Task TestWindowFocusOutEvent(CancellationToken ct = default) {
        // Arrange
        int focusOutEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterFocusOutHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref focusOutEventCount);
                })
            , ct
        );

        // Ensure the window is focused before recording the baseline,
        // so that minimizing it produces a clean FocusOut transition
        windowUtility.Window.SetFocused();
        await Task.Delay(100, ct);
        int baseline = Volatile.Read(ref focusOutEventCount);

        // Act: minimize causes WM_ACTIVATE with WA_INACTIVE → FocusOut
        windowUtility.Window.SetMinimized(true);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref focusOutEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(focusOutEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
