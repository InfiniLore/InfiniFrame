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
    private const ushort ImageFileMachineAmd64 = 0x8664;
    private const ushort ImageFileMachineArm64 = 0xAA64;

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
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName),
            ImageFileMachineAmd64
        );

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
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName),
            ImageFileMachineAmd64
        );
        WriteMinimalPeBinary(
            Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName),
            ImageFileMachineAmd64
        );

        // Act
        NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "win-x64");

        // Assert
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName))).IsTrue();
        await Assert.That(File.Exists(Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName))).IsTrue();
    }

    [Test]
    public async Task ValidateArtifacts_Throws_WhenWindowsArtifactArchitectureMismatchesRid() {
        // Arrange
        string artifactsDirectory = TemporaryDirectory.Path;
        string nativeDll = Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsNativeFileName);
        string loaderDll = Path.Join(artifactsDirectory, InfiniFrameNativeArtifactManifest.WindowsLoaderFileName);
        WriteMinimalPeBinary(nativeDll, ImageFileMachineArm64);
        WriteMinimalPeBinary(loaderDll, ImageFileMachineArm64);

        // Act & Assert
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => {
            NativeRuntimeBuilder.ValidateArtifacts(artifactsDirectory, "win-x64");
            return Task.CompletedTask;
        }) ?? throw new InvalidOperationException("Expected exception was not thrown.");

        await Assert.That(ex.Message).Contains("architecture mismatch");
        await Assert.That(ex.Message).Contains("Expected x64");
        await Assert.That(ex.Message).Contains("found arm64");
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
}
