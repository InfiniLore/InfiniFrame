// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Taskbar;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TaskbarProgressTests {

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetProgress(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100);

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.Normal);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetProgress_Indeterminate(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Indeterminate, 0, 0);

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.Indeterminate);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetProgress_Error(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Error, 75, 100);

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.Error);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_SetProgress_Paused(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Paused, 30, 100);

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.Paused);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_ClearProgress(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100);

        // Act
        window.Features.Taskbar.ClearProgress();

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.None);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionSetProgress(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetTaskbarProgress(TaskbarProgressState.Normal, 50, 100);

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.Normal);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionClearProgress(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100);

        // Act
        IInfiniFrameWindow returnedWindow = window.ClearTaskbarProgress();

        // Assert
        await Assert.That(window.Features.Taskbar.CurrentProgressState).IsEqualTo(TaskbarProgressState.None);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
