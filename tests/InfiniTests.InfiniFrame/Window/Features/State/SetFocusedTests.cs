// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.State;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetFocusedTests {
    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.State.SetFocused();

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetFocused();

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
