// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatingEventTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtBuilderStage_EventFiresOnce(CancellationToken ct = default) {
        // Arrange
        int creatingEventCount = 0;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var _ = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowCreatingHandler(_ => {
            Interlocked.Increment(ref creatingEventCount);
            eventRaised.TrySetResult(true);
        }), ct);

        // Assert: event fires during window creation.
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(creatingEventCount).IsEqualTo(1);
    }
}
