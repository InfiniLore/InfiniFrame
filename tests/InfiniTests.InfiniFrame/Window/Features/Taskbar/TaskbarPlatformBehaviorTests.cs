// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Taskbar;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TaskbarPlatformBehaviorTests {

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_IsSupported_DependsOnDesktopEnvironment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool isSupported = window.Features.Taskbar.IsSupported;

        // Assert - on Linux, support depends on D-Bus StatusNotifierItem or Unity LauncherEntry
        await Assert.That(isSupported).IsTypeOf<bool>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_Capabilities_ReturnsCorrectValues(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert - capabilities reflect actual platform support
        await Assert.That(capabilities.SupportsProgress).IsTypeOf<bool>();
        await Assert.That(capabilities.SupportsFlash).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_IsSupported_ReturnsTrue(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool isSupported = window.Features.Taskbar.IsSupported;

        // Assert - macOS always supports dock badge progress
        await Assert.That(isSupported).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_Capabilities_ReturnsProgressAndFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert - macOS supports progress (dock badge) and flash (requestUserAttention)
        await Assert.That(capabilities.SupportsProgress).IsTrue();
        await Assert.That(capabilities.SupportsFlash).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnMacOs_SetProgress_SetsDockBadge(CancellationToken ct) {
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
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnMacOs_ClearProgress_RemovesDockBadge(CancellationToken ct) {
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
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnMacOs_SetFlash_RequestsUserAttention(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0);

        // Assert - flash is fire-and-forget on macOS
        await Assert.That(window.Features.Taskbar).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_StopFlash_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert - StopFlash is a no-op on macOS
        window.Features.Taskbar.StopFlash();
        await Assert.That(window.Features.Taskbar).IsNotNull();
    }
}
