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
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_IsSupported_ReturnsFalse(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool isSupported = window.Features.Taskbar.IsSupported;

        // Assert
        await Assert.That(isSupported).IsFalse();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_Capabilities_ReturnsNotSupported(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert
        await Assert.That(capabilities.SupportsProgress).IsFalse();
        await Assert.That(capabilities.SupportsFlash).IsFalse();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_IsSupported_ReturnsFalse(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool isSupported = window.Features.Taskbar.IsSupported;

        // Assert
        await Assert.That(isSupported).IsFalse();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_Capabilities_ReturnsNotSupported(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert
        await Assert.That(capabilities.SupportsProgress).IsFalse();
        await Assert.That(capabilities.SupportsFlash).IsFalse();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_SetProgress_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100))
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_ClearProgress_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.ClearProgress())
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_SetFlash_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0))
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnLinux_StopFlash_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.StopFlash())
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_SetProgress_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.SetProgress(TaskbarProgressState.Normal, 50, 100))
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_ClearProgress_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.ClearProgress())
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_SetFlash_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.SetFlash(TaskbarFlashMode.All, 0))
            .ThrowsExactly<PlatformNotSupportedException>();
    }

    [Test]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_StopFlash_ThrowsPlatformNotSupportedException(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(() => window.Features.Taskbar.StopFlash())
            .ThrowsExactly<PlatformNotSupportedException>();
    }
}
