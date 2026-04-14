// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using InfiniFrameTests.Tools.Pack.TestUtilities;

namespace InfiniFrameTests.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PublishValidatorTests {
    private const ushort ImageFileMachineAmd64 = 0x8664;
    private const ushort ImageFileMachineArm64 = 0xAA64;

    private TemporaryDirectory TemporaryDirectory { get; set; } = null!;
    
    private static void WriteMinimalPeBinary(string path, ushort machine) {
        byte[] bytes = new byte[0x90];
        bytes[0] = (byte)'M';
        bytes[1] = (byte)'Z';

        // e_lfanew points to the PE signature location.
        bytes[0x3C] = 0x80;
        bytes[0x3D] = 0x00;
        bytes[0x3E] = 0x00;
        bytes[0x3F] = 0x00;

        bytes[0x80] = (byte)'P';
        bytes[0x81] = (byte)'E';
        bytes[0x82] = 0x00;
        bytes[0x83] = 0x00;

        // IMAGE_FILE_HEADER.Machine
        bytes[0x84] = (byte)(machine & 0xFF);
        bytes[0x85] = (byte)((machine >> 8) & 0xFF);

        File.WriteAllBytes(path, bytes);
    }
    
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
    public async Task ValidateNativeArtifacts_Throws_WhenArtifactsDirectoryIsMissing() {
        // Arrange
        string missingDirectory = Path.Join(Path.GetTempPath(), $"missing-artifacts-{Guid.NewGuid():N}");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateNativeArtifacts(missingDirectory, "win-x64");
                return Task.CompletedTask;
            })
            .WithMessage($"Native artifacts directory was not found: {missingDirectory}");
    }

    [Test]
    public async Task ValidateNativeArtifacts_Throws_WhenWindowsRequiredArtifactIsMissing() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsNativeFileName),
            ImageFileMachineAmd64
        );

        // Act & Assert
        string expectedMissingFile = Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName);
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "win-x64");
                return Task.CompletedTask;
            })
            .WithMessage($"Required native artifact was not found: {expectedMissingFile}");
    }

    [Test]
    public async Task ValidateNativeArtifacts_DoesNotThrow_ForWindowsWhenAllRequiredArtifactsExist() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsNativeFileName),
            ImageFileMachineAmd64
        );
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName),
            ImageFileMachineAmd64
        );

        // Act
        PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "win-x64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsNativeFileName))).IsTrue();
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateNativeArtifacts_Throws_WhenWindowsArtifactArchitectureMismatchesRid() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        string nativeDll = Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsNativeFileName);
        string loaderDll = Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.WindowsLoaderFileName);
        WriteMinimalPeBinary(nativeDll, ImageFileMachineArm64);
        WriteMinimalPeBinary(loaderDll, ImageFileMachineArm64);

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "win-x64");
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("architecture mismatch");
        await Assert.That(ex.Message).Contains("Expected x64");
        await Assert.That(ex.Message).Contains("found arm64");
    }

    [Test]
    public async Task ValidateNativeArtifacts_DoesNotThrow_ForLinuxWhenRequiredArtifactExists() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.LinuxNativeFileName), string.Empty);

        // Act
        PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "linux-x64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.LinuxNativeFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateNativeArtifacts_DoesNotThrow_ForOsxWhenRequiredArtifactExists() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        await File.WriteAllTextAsync(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.OsxNativeFileName), string.Empty);

        // Act
        PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "osx-arm64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFramePackNativeArtifactManifest.OsxNativeFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateNativeArtifacts_Throws_WhenRidIsUnsupported() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateNativeArtifacts(artifactsDirectory, "browser-wasm");
                return Task.CompletedTask;
            })
            .WithMessage("Unsupported RID for native artifact validation: browser-wasm");
    }

    [Test]
    public async Task ValidateRidConsistency_Throws_WhenRidIsEmpty() {
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateRidConsistency(string.Empty);
                return Task.CompletedTask;
            })
            .WithMessage("Runtime identifier (RID) cannot be empty.");
    }

    [Test]
    public async Task ValidateRidConsistency_Throws_WhenRidFormatIsInvalid() {
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateRidConsistency("linuxx64");
                return Task.CompletedTask;
            })
            .WithMessage("Invalid RID format: 'linuxx64'. Expected format like 'win-x64', 'linux-arm64'.");
    }

    [Test]
    public async Task ValidateRidConsistency_Throws_WhenRidIsUnsupported() {
        await Assert.ThrowsAsync<InvalidOperationException>(() => {
                PublishValidator.ValidateRidConsistency("browser-wasm");
                return Task.CompletedTask;
            })
            .WithMessage("Unsupported or unknown RID: 'browser-wasm'.");
    }

    [Test]
    public async Task ValidateRidConsistency_ReturnsTrue_ForSupportedRid() {
        bool output = PublishValidator.ValidateRidConsistency("linux-x64");
        await Assert.That(output).IsTrue();
    }
    
    [Test]
    public async Task ValidateOutputPath_AllowsProjectBinPath() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(projectDirectory, "bin", "Release", "net10.0", "win-x64", "publish");

        bool output = PublishValidator.ValidateOutputPath(outputPath, projectDirectory, forceCleanOutput: false);
        await Assert.That(output).IsTrue();
    }

    [Test]
    public async Task ValidateOutputPath_ThrowsForNonDefaultPath_WhenNotForced() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(TemporaryDirectory.Path, "publish-output");

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            PublishValidator.ValidateOutputPath(outputPath, projectDirectory, forceCleanOutput: false);
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("--force-clean-output");
    }

    [Test]
    public async Task ValidateOutputPath_AllowsNonDefaultPath_WhenForced() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(TemporaryDirectory.Path, "publish-output");

        bool output = PublishValidator.ValidateOutputPath(outputPath, projectDirectory, forceCleanOutput: true);
        await Assert.That(output).IsTrue();
    }

    [Test]
    public async Task ValidateOutputPath_RejectsCaseMismatchForBinDirectory_OnCaseSensitivePlatforms() {
        string projectDirectory = Path.Join(TemporaryDirectory.Path, "app");
        string outputPath = Path.Join(projectDirectory, "BIN", "Release", "net10.0", "win-x64", "publish");

        if (OperatingSystem.IsWindows()) {
            bool output = PublishValidator.ValidateOutputPath(outputPath, projectDirectory, forceCleanOutput: false);
            await Assert.That(output).IsTrue();
            return;
        }

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            PublishValidator.ValidateOutputPath(outputPath, projectDirectory, forceCleanOutput: false);
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("--force-clean-output");
    }
}
