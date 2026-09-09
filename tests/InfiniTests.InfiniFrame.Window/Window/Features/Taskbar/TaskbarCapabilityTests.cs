// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Taskbar;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TaskbarCapabilityTests {

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task OnWindows_IsSupported_ReturnsTrue(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool isSupported = window.Features.Taskbar.IsSupported;

        // Assert
        await Assert.That(isSupported).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task OnWindows_Capabilities_HasProgressAndFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert
        await Assert.That(capabilities.SupportsProgress).IsTrue();
        await Assert.That(capabilities.SupportsFlash).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    [SkipOnMacOs]
    public async Task OnWindows_IsSupported_CachesResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool first = window.Features.Taskbar.IsSupported;
        bool second = window.Features.Taskbar.IsSupported;

        // Assert
        await Assert.That(first).IsEqualTo(second);
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

        // Assert
        await Assert.That(isSupported).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnLinux]
    public async Task OnMacOs_Capabilities_HasProgressAndFlash(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        InfiniFrameTaskbarCapabilities capabilities = window.Features.Taskbar.Capabilities;

        // Assert
        await Assert.That(capabilities.SupportsProgress).IsTrue();
        await Assert.That(capabilities.SupportsFlash).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnWindows]
    [SkipOnMacOs]
    public async Task OnMacOs_IsSupported_CachesResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        bool first = window.Features.Taskbar.IsSupported;
        bool second = window.Features.Taskbar.IsSupported;

        // Assert
        await Assert.That(first).IsEqualTo(second);
    }
}
