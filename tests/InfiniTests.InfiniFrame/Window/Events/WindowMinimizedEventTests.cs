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
    [SkipOnLinux("desktop-state dependent under WSLg/local Linux runs")]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetMinimized_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int minimizedEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterMinimizedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref minimizedEventCount);
            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);
        baseline = Volatile.Read(ref minimizedEventCount);

        // Act
        windowUtility.Window.SetMinimized();

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(minimizedEventCount).IsEqualTo(baseline + 1);
    }
}