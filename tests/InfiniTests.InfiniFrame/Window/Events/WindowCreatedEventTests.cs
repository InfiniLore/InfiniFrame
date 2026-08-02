// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowCreatedEventTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task AtBuilderStage_EventFiresOnce(CancellationToken ct = default) {
        // Arrange
        int createdEventCount = 0;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var _ = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowCreatedHandler(_ => {
            Interlocked.Increment(ref createdEventCount);
            eventRaised.TrySetResult(true);
        }), ct);

        // Assert: event fires during window creation.
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(createdEventCount).IsEqualTo(1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtBuilderStage_SendWebMessageInsideHandler_DoesNotCrash(CancellationToken ct = default) {
        // Arrange
        bool windowCreatedCalled = false;
        using var _ = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowCreatedHandler(window => {
            window.SendWebMessage("hello-from-window-created");

            // ReSharper disable once AccessToModifiedClosure
            Volatile.Write(ref windowCreatedCalled, true);
        }), ct);

        // Assert
        await Assert.That(Volatile.Read(ref windowCreatedCalled)).IsTrue();
    }
}