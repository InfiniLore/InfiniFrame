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
        int closingCounter = 0;
        var window1Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCounter);
                return false;
            }), ct);
        var window2Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCounter);
                return false;
            }), ct);

        // Act
        using (window1Utility) {
            IInfiniFrameWindow window1 = window1Utility.Window;
            window1.Close();
        }

        using (window2Utility) {
            IInfiniFrameWindow window2 = window2Utility.Window;
            window2.Close();
        }

        // Assert
        await Assert.That(closingCounter).IsEqualTo(2);

    }
}
