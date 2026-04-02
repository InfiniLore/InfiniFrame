// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using System.Runtime.InteropServices;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RuntimeResolverTests {
    [Test]
    public async Task ResolveRid_ReturnsRequestedRid_WhenNotAuto() {
        // Arrange
        const string requestedRid = "linux-arm64";

        // Act
        string rid = RuntimeResolver.ResolveRid(requestedRid);

        // Assert
        await Assert.That(rid).IsEqualTo(requestedRid);
    }

    [Test]
    public async Task ResolveRid_ReturnsCurrentPlatformRid_WhenAutoIsRequested() {
        // Arrange
        const string requestedRid = "auto";
        string expectedPrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "win-"
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? "linux-"
                : "osx-";

        // Act
        string rid = RuntimeResolver.ResolveRid(requestedRid);

        // Assert
        await Assert.That(rid).StartsWith(expectedPrefix);
        await Assert.That(rid).Matches("^(win|linux|osx)-(x64|arm64)$");
    }

    [Test]
    public async Task ResolveNativeOsDir_ReturnsWindows_ForWindowsRid() {
        // Arrange
        const string rid = "WIN-x64";

        // Act
        string osDirectory = RuntimeResolver.ResolveNativeOsDir(rid);

        // Assert
        await Assert.That(osDirectory).IsEqualTo("windows");
    }

    [Test]
    public async Task ResolveNativeOsDir_ReturnsLinux_ForLinuxRid() {
        // Arrange
        const string rid = "linux-arm64";

        // Act
        string osDirectory = RuntimeResolver.ResolveNativeOsDir(rid);

        // Assert
        await Assert.That(osDirectory).IsEqualTo("linux");
    }

    [Test]
    public async Task ResolveNativeOsDir_ReturnsOsx_ForOsxRid() {
        // Arrange
        const string rid = "osx-x64";

        // Act
        string osDirectory = RuntimeResolver.ResolveNativeOsDir(rid);

        // Assert
        await Assert.That(osDirectory).IsEqualTo("osx");
    }

    [Test]
    public async Task ResolveNativeOsDir_Throws_ForUnsupportedRid() {
        // Arrange
        const string rid = "android-arm64";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                RuntimeResolver.ResolveNativeOsDir(rid);
                return Task.CompletedTask;
            })
            .WithMessage("Unsupported RID for native artifact resolution: android-arm64");
    }

    [Test]
    public async Task ResolveNativePlatform_ReturnsArm64_WhenRidContainsArm64() {
        // Arrange
        const string rid = "linux-arm64";

        // Act
        string platform = RuntimeResolver.ResolveNativePlatform(rid);

        // Assert
        await Assert.That(platform).IsEqualTo("arm64");
    }

    [Test]
    public async Task ResolveNativePlatform_ReturnsX64_WhenRidDoesNotContainArm64() {
        // Arrange
        const string rid = "linux-x64";

        // Act
        string platform = RuntimeResolver.ResolveNativePlatform(rid);

        // Assert
        await Assert.That(platform).IsEqualTo("x64");
    }
}
