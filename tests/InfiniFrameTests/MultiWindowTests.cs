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
    [Timeout(TimeoutUtility.DefaultTimeout * 3)]
    public async Task OpenWindowAfterOneCloses(CancellationToken ct) {
        // Arrange
        int closingCounter = 0;
        var window1Closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var window2Closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var window1Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCounter);
                window1Closed.TrySetResult();
                return false;
            }), ct);

        // Act
        using (window1Utility) {
            IInfiniFrameWindow window1 = window1Utility.Window;
            window1.Close();
        }
        await window1Closed.Task.WaitAsync(ct);

        var window2Utility = InfiniFrameWindowTestUtility.Create(
            builder => builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCounter);
                window2Closed.TrySetResult();
                return false;
            }), ct);

        using (window2Utility) {
            IInfiniFrameWindow window2 = window2Utility.Window;
            window2.Close();
        }
        await window2Closed.Task.WaitAsync(ct);

        // Assert
        await Assert.That(closingCounter).IsEqualTo(2);

    }
}
