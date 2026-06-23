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
    public async Task AtWindowStage_SetMaximized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int maximizedEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterMaximizedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref maximizedEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);
        baseline = Volatile.Read(ref maximizedEventCount);

        // Act
        windowUtility.Window.SetMaximized();

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(maximizedEventCount).IsEqualTo(baseline + 1);
    }
}
