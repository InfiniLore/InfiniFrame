// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Taskbar;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TaskbarFlashTests {

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0);

        // Assert - flash is fire-and-forget, verify feature is accessible
        await Assert.That(window.Features.Taskbar).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetFlash_Timer(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetFlash(TaskbarFlashMode.Timer, 3);

        // Assert
        await Assert.That(window.Features.Taskbar).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_StopFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0);

        // Act
        window.Features.Taskbar.StopFlash();

        // Assert
        await Assert.That(window.Features.Taskbar).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.FlashTaskbar(TaskbarFlashMode.All);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionStopFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0);

        // Act
        IInfiniFrameWindow returnedWindow = window.StopTaskbarFlash();

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
