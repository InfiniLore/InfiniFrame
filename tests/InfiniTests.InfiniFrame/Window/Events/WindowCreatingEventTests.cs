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
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtBuilderStage_EventFiresOnce(CancellationToken ct = default) {
        // Arrange
        int creatingEventCount = 0;
        using var _ = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowCreatingHandler(_ => {
            Interlocked.Increment(ref creatingEventCount);
        }), ct);

        // Assert: event fires during window creation.
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref creatingEventCount), 0, TimeSpan.FromSeconds(5), ct);
        await Assert.That(creatingEventCount).IsEqualTo(1);
    }
}
