// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NativeRuntimeBuilderTests {
    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Setup
    // -----------------------------------------------------------------------------------------------------------------
    [Before(Test)]
    public void Before() {
        TemporaryDirectory = TemporaryDirectory.Create();
    }

    [After(Test)]
    public void After() {
        TemporaryDirectory.Dispose();
        TemporaryDirectory = null!;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ValidateArtifacts_Throws_WhenArtifactsDirectoryIsMissing() {
        // Arrange
        string missingDirectory = Path.Join(Path.GetTempPath(), $"missing-artifacts-{Guid.NewGuid():N}");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                NativeRuntimeBuilder.ValidateArtifacts(missingDirectory, "win-x64");
                return Task.CompletedTask;
            })
            .WithMessage($"Native artifacts directory was not found: {missingDirectory}");
    }

    [Test]
    public async Task ValidateArtifacts_Throws_WhenWindowsRequiredArtifactIsMissing() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName), string.Empty);

        // Act & Assert
        string expectedMissingFile = Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName);
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "win-x64");
                return Task.CompletedTask;
            })
            .WithMessage($"Required native artifact was not found: {expectedMissingFile}");
    }

    [Test]
    public async Task ValidateArtifacts_DoesNotThrow_ForWindowsWhenAllRequiredArtifactsExist() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName), string.Empty);
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName), string.Empty);

        // Act
        NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "win-x64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName))).IsTrue();
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateArtifacts_DoesNotThrow_ForLinuxWhenRequiredArtifactExists() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.LinuxNativeFileName), string.Empty);

        // Act
        NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "linux-x64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.LinuxNativeFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateArtifacts_DoesNotThrow_ForOsxWhenRequiredArtifactExists() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.OsxNativeFileName), string.Empty);

        // Act
        NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "osx-arm64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.OsxNativeFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateArtifacts_Throws_WhenRidIsUnsupported() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "browser-wasm");
                return Task.CompletedTask;
            })
            .WithMessage("Unsupported RID for native artifact validation: browser-wasm");
    }
}
