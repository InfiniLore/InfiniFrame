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
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_SetSize_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int sizeChangedCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
            .SetSize(800, 600)
            .RegisterSizeChangedHandler((_, _) => {
                // ReSharper disable once AccessToModifiedClosure
                int current = Interlocked.Increment(ref sizeChangedCount);
                
                // ReSharper disable once AccessToModifiedClosure
                if (current > Volatile.Read(ref baseline)) {
                    eventRaised.TrySetResult(true);
                }
            }), ct);
        baseline = Volatile.Read(ref sizeChangedCount);

        // Act
        windowUtility.Window.SetSize(400, 300);

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(sizeChangedCount).IsGreaterThanOrEqualTo(baseline + 1);
    }
}
