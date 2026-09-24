// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosedEventTests {
    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    public async Task OnMacOs_PooledHost_DoesNotInvokePriorSessionClosedCallback(CancellationToken ct = default) {
        int firstClosed = 0;
        using (var first = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowClosedHandler(_ => firstClosed++), ct)) {
            first.Window.Close();
            first.Window.WaitForClose();
        }

        using var second = InfiniFrameTestWindow.Create(ct);
        second.Window.Close();
        second.Window.WaitForClose();
        await Assert.That(firstClosed).IsEqualTo(1);
    }
    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_Close_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowClosedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref closedEventCount);

            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        }), ct);
        baseline = Volatile.Read(ref closedEventCount);

        // Act
        windowUtility.Window.Close();

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closedEventCount).IsEqualTo(baseline + 1);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_Close_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        int baseline = int.MaxValue;
        TaskCompletionSource<bool> eventRaised = PollUtility.CreateSignal();
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterWindowClosedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            int current = Interlocked.Increment(ref closedEventCount);

            // ReSharper disable once AccessToModifiedClosure
            if (current > Volatile.Read(ref baseline)) {
                eventRaised.TrySetResult(true);
            }
        });
        baseline = Volatile.Read(ref closedEventCount);

        // Act
        window.Close();

        // Assert
        await PollUtility.WaitForSignalAsync(eventRaised, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closedEventCount).IsEqualTo(baseline + 1);
    }
}
