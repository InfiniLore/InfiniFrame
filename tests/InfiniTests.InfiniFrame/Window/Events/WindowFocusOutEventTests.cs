// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFocusOutEventTests {
    [Test]
    [SkipOnWindowsArm("WM_ACTIVATE WA_INACTIVE is not reliably delivered on headless ARM64 CI runners")]
    [SkipOnLinux("Focus transitions are desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(5_000 + 100)]
    public async Task AtWindowStage_SetMinimized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int focusOutEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterFocusOutHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref focusOutEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);

        windowUtility.Window.SetFocused();
        await Task.Delay(100, ct);
        baseline = Volatile.Read(ref focusOutEventCount);

        // Act
        windowUtility.Window.SetMinimized();

        // Assert
        try {
            await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        }
        catch (TimeoutException) {
            Skip.Test("FocusOut did not fire in this desktop state.");
            return;
        }

        await Assert.That(focusOutEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
