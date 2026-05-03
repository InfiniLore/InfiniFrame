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
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task OpenWindowAfterOneCloses(CancellationToken ct) {
        // Arrange
        int closingRequestedCounter = 0;
        var window1Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingRequestedHandler(_ => {
                Interlocked.Increment(ref closingRequestedCounter);
            }), ct);

        // Act
        window1Utility.Dispose();

        var window2Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingRequestedHandler(_ => {
                Interlocked.Increment(ref closingRequestedCounter);
            }), ct);

        window2Utility.Dispose();

        // Assert
        await Assert.That(closingRequestedCounter).IsEqualTo(2);
    }
}
