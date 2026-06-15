// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Events;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowClosingRequestedEventTests {
    [Test]
    [Retry(5)]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_Close_RaisesEvent(CancellationToken ct = default) {
        // Arrange
        int closingRequestedEventCount = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder.RegisterWindowClosingRequestedHandler(_ => {
            // ReSharper disable once AccessToModifiedClosure
            Interlocked.Increment(ref closingRequestedEventCount);
        }), ct);
        int baseline = Volatile.Read(ref closingRequestedEventCount);

        // Act
        windowUtility.Window.Close();

        // Assert
        await PollUtility.WaitForChangeAsync(() => Volatile.Read(ref closingRequestedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(closingRequestedEventCount).IsEqualTo(baseline + 1);
    }
}
