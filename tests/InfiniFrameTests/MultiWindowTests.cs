// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MultiWindowTests {
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    // [Timeout(TimeoutUtility.DefaultTimeout + 1_000)]
    public async Task OpenWindowAfterOneCloses(CancellationToken ct) {
        // Arrange
        int closingRequestedCounter = 0;
        var window1Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingRequestedHandler(_ => {
                Interlocked.Increment(ref closingRequestedCounter);
            }), ct);
        window1Utility.Dispose(); // cleans up and closes the window
        await Task.Delay(1_000, ct);
        
        // Act
        var window2Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingRequestedHandler(_ => {
                Interlocked.Increment(ref closingRequestedCounter);
            }), ct);

        window2Utility.Dispose(); // cleans up and closes the window

        // Assert
        await Assert.That(closingRequestedCounter).IsEqualTo(2);

    }
}
