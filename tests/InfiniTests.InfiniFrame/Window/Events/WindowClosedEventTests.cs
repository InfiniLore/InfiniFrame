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
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_Close_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowClosedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref closedEventCount);
        }), ct);
        int baseline = Volatile.Read(ref closedEventCount);

        // Act
        windowUtility.Window.Close();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref closedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closedEventCount).IsEqualTo(baseline + 1);
    }

    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_Close_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int closedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterWindowClosedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref closedEventCount);
        });
        int baseline = Volatile.Read(ref closedEventCount);

        // Act
        window.Close();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref closedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closedEventCount).IsEqualTo(baseline + 1);
    }
}
