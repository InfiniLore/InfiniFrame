// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ToggleMaximizedTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool initialValue = window.Features.State.IsMaximized;

        // Act
        window.Features.State.ToggleMaximized();

        // Assert
        await Assert.That(window.Features.State.IsMaximized).IsEqualTo(!initialValue);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        bool initialValue = window.Features.State.IsMaximized;

        // Act
        IInfiniFrameWindow returnedWindow = window.ToggleMaximized();

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.Features.State.IsMaximized).IsEqualTo(!initialValue);
    }
}
