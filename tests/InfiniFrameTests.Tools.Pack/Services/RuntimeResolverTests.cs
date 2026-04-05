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

}
