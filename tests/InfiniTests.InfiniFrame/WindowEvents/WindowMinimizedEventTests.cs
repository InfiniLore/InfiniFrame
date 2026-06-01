// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowEvents;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowMinimizedEventTests {
    [Test, Retry(5), SkipOnMacOs, SkipOnLinux("desktop-state dependent under WSLg/local Linux runs"), NotInParallelInfiniTests]
    public async Task TestWindowMinimizedEvent(CancellationToken ct = default) {
        // Arrange
        int minimizedEventCount = 0;
        int baseline = Volatile.Read(ref minimizedEventCount);

        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => builder
                .RegisterMinimizedHandler(_ => {
                    // ReSharper disable once AccessToModifiedClosure
                    Interlocked.Increment(ref minimizedEventCount);
                })
            , ct
        );

        // Act
        windowUtility.Window.SetMinimized(true);

        // Assert
        await PollUtility.WaitForChangeAsync(getValue: () => Volatile.Read(ref minimizedEventCount), baseline, TimeSpan.FromSeconds(5), ct);
        await Assert.That(minimizedEventCount).IsEqualTo(baseline + 1);
    }
}
