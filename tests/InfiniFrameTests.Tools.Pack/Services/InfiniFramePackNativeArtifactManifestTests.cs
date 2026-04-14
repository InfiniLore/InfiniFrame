// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFramePackNativeArtifactManifestTests {
    [Test]
    public async Task RequiredFileNamesForRid_ReturnsWindowsArtifacts_ForWindowsRid() {
        // Act
        string[] required = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid("win-x64");

        // Assert
        await Assert.That(required).IsEquivalentTo([
            InfiniFramePackNativeArtifactManifest.WindowsNativeFileName,
            InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName
        ]);
    }

    [Test]
    public async Task RequiredFileNamesForRid_ReturnsLinuxArtifact_ForLinuxRid() {
        // Act
        string[] required = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid("linux-arm64");

        // Assert
        await Assert.That(required).IsEquivalentTo([
            InfiniFramePackNativeArtifactManifest.LinuxNativeFileName
        ]);
    }

    [Test]
    public async Task RequiredFileNamesForRid_ReturnsOsxArtifact_ForOsxRid() {
        // Act
        string[] required = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid("osx-arm64");

        // Assert
        await Assert.That(required).IsEquivalentTo([
            InfiniFramePackNativeArtifactManifest.OsxNativeFileName
        ]);
    }

    [Test]
    public async Task RequiredFileNamesForRid_MatchesRidPrefix_CaseInsensitively() {
        // Act
        string[] required = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid("WIN-X64");

        // Assert
        await Assert.That(required).IsEquivalentTo([
            InfiniFramePackNativeArtifactManifest.WindowsNativeFileName,
            InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName
        ]);
    }

    [Test]
    public async Task RequiredFileNamesForRid_Throws_WhenRidIsUnsupported() {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid("browser-wasm");
                return Task.CompletedTask;
            })
            .WithMessage("Unsupported RID for native artifact validation: browser-wasm");
    }

    [Test]
    public async Task RidArtifacts_ContainsExpectedRidToFileMappings() {
        // Assert
        await Assert.That(InfiniFramePackNativeArtifactManifest.RidArtifacts).IsEquivalentTo([
            new InfiniFramePackNativeArtifactManifest.NativeRidArtifact("win-", InfiniFramePackNativeArtifactManifest.WindowsNativeFileName),
            new InfiniFramePackNativeArtifactManifest.NativeRidArtifact("win-", InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName),
            new InfiniFramePackNativeArtifactManifest.NativeRidArtifact("linux-", InfiniFramePackNativeArtifactManifest.LinuxNativeFileName),
            new InfiniFramePackNativeArtifactManifest.NativeRidArtifact("osx-", InfiniFramePackNativeArtifactManifest.OsxNativeFileName)
        ]);
    }

    [Test]
    public async Task AllFileNames_ContainsExpectedNativeArtifactFileNames() {
        // Assert
        await Assert.That(InfiniFramePackNativeArtifactManifest.AllFileNames).IsEquivalentTo([
            InfiniFramePackNativeArtifactManifest.WindowsNativeFileName,
            InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName,
            InfiniFramePackNativeArtifactManifest.LinuxNativeFileName,
            InfiniFramePackNativeArtifactManifest.OsxNativeFileName
        ]);
    }
}
