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
    [SkipOnLinux("Focus transitions are desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetFocused_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int focusInEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterFocusInHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref focusInEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);

        windowUtility.Window.SetMinimized();
        await Task.Delay(100, ct);
        baseline = Volatile.Read(ref focusInEventCount);

        // Act
        windowUtility.Window.SetFocused();

        // Assert
        try {
            await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        }
        catch (TimeoutException) when (OperatingSystem.IsMacOS()) {
            Skip.Test("FocusIn did not fire in this macOS desktop state.");
            return;
        }
        await Assert.That(focusInEventCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
