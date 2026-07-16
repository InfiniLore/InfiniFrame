// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Invoke;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InvokeTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int callbackThreadId = -1;

        // Act
        window.Features.Invoke.Invoke(() => {
            callbackThreadId = Environment.CurrentManagedThreadId;
        });

        // Assert
        await Assert.That(callbackThreadId).IsEqualTo(window.ManagedThreadId);
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int callbackThreadId = -1;

        // Act
        IInfiniFrameWindow returnedWindow = window.Invoke(() => {
            callbackThreadId = Environment.CurrentManagedThreadId;
        });

        // Assert
        await Assert.That(callbackThreadId).IsEqualTo(window.ManagedThreadId);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
