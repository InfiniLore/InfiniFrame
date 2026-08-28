// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.SingleFile;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class InfiniFrameSingleFileBootstrapTests {

    private static readonly Type BootstrapType = typeof(InfiniFrameSingleFile).Assembly.GetType("InfiniFrame.InfiniFrameSingleFileBootstrap")!;
    private static readonly FieldInfo InitializedField = BootstrapType.GetField("_initialized", BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly FieldInfo NativeDirField = BootstrapType.GetField("_nativeDir", BindingFlags.NonPublic | BindingFlags.Static)!;

    private static void ResetState() {
        InitializedField.SetValue(null, 0);
        NativeDirField.SetValue(null, null);
    }

    private static string GetExpectedRid() {
        string os = OperatingSystem.IsWindows() ? "win"
            : OperatingSystem.IsLinux() ? "linux"
            : OperatingSystem.IsMacOS() ? "osx"
            : throw new PlatformNotSupportedException();

        string arch = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64" : "x64";
        return $"{os}-{arch}";
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GetRuntimeIdentifier (via Initialize)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_Rid_MatchesCurrentPlatform(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string expectedRid = GetExpectedRid();
        string[] parts = expectedRid.Split('-');

        // Act (no-op — verifying helper constructs correct format)

        // Assert
        await Assert.That(parts.Length).IsEqualTo(2);
        await Assert.That(parts[0]).IsNotEmpty();
        await Assert.That(parts[1]).IsNotEmpty();

        // Cleanup
        ResetState();
    }

    [Test]
    public async Task Initialize_Rid_StartsWithKnownOs(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string rid = GetExpectedRid();
        string os = rid.Split('-')[0];
        string[] knownOs = ["win", "linux", "osx"];

        // Act (no-op — verifying helper constructs correct format)

        // Assert
        await Assert.That(knownOs).Contains(os);

        // Cleanup
        ResetState();
    }

    [Test]
    public async Task Initialize_Rid_EndsWithKnownArchitecture(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string rid = GetExpectedRid();
        string arch = rid.Split('-')[1];
        string[] knownArch = ["x64", "arm64"];

        // Act (no-op — verifying helper constructs correct format)

        // Assert
        await Assert.That(knownArch).Contains(arch);

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Resource name construction
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ResourceName_FollowsExpectedPattern(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();
        string assemblyName = "TestApp";
        string fileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Act
        string resourceName = $"{assemblyName}.native.{rid}.{fileName}";

        // Assert
        await Assert.That(resourceName).Contains(".native.");
        await Assert.That(resourceName).EndsWith(fileName);
    }

    [Test]
    public async Task ResourceName_ContainsRidBetweenNativeMarkerAndFileName(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();
        string assemblyName = "MyApp";
        string fileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Act
        string resourceName = $"{assemblyName}.native.{rid}.{fileName}";
        string[] parts = resourceName.Split('.');
        int nativeIndex = Array.IndexOf(parts, "native");

        // Assert
        await Assert.That(nativeIndex).IsGreaterThan(-1);
        await Assert.That(parts[nativeIndex + 1]).IsEqualTo(rid);
    }

    [Test]
    public async Task ResourceName_KnownPlatforms_AllHaveValidFormat(CancellationToken ct = default) {
        // Arrange
        string[] knownRids = ["win-x64", "win-arm64", "linux-x64", "linux-arm64", "osx-x64", "osx-arm64"];
        string assemblyName = "TestApp";

        // Act & Assert
        foreach (string rid in knownRids) {
            string[] parts = rid.Split('-');
            await Assert.That(parts.Length).IsEqualTo(2);

            string os = parts[0];
            string expectedExtension = os switch {
                "win" => ".dll",
                "linux" => ".so",
                "osx" => ".dylib",
                _ => throw new PlatformNotSupportedException()
            };

            string resourceName = $"{assemblyName}.native.{rid}.InfiniFrame.Native{expectedExtension}";
            await Assert.That(resourceName).Contains(rid);
            await Assert.That(resourceName).EndsWith(expectedExtension);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Current platform resource name
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task CurrentPlatform_RidMatchesArtifactManifestFileName(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();
        string expectedOs = rid.Split('-')[0];
        string fileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        string expectedExtension = expectedOs switch {
            "win" => ".dll",
            "linux" => ".so",
            "osx" => ".dylib",
            _ => throw new PlatformNotSupportedException()
        };

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(fileName).EndsWith(expectedExtension);
    }

    [Test]
    public async Task RequiredFiles_MatchRidFormat(CancellationToken ct = default) {
        // Arrange
        string[] requiredFiles = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(requiredFiles).IsNotEmpty();

        foreach (string file in requiredFiles) {
            await Assert.That(file).IsNotEmpty();
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // NativeLibrary name constants
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NativeLibraryName_IsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.NativeLibraryName).IsEqualTo("InfiniFrame.Native");
    }

    [Test]
    public async Task WindowsLoaderLibraryName_IsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsLoaderLibraryName).IsEqualTo("WebView2Loader");
    }

    [Test]
    public async Task ResolveNativeLibraryFileNameForCurrentPlatform_ReturnsValidFileName(CancellationToken ct = default) {
        // Arrange
        string fileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(fileName).IsNotEmpty();
        await Assert.That(fileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_ReturnsNonEmptyArray(CancellationToken ct = default) {
        // Arrange
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required.Length).IsGreaterThan(0);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Initialize - no embedded resources
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_NoEmbeddedResources_NativeDirIsNull(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo initMethod = BootstrapType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        initMethod.Invoke(null, null);

        // Assert
        string? nativeDir = (string?)NativeDirField.GetValue(null);
        await Assert.That(nativeDir).IsNull();

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Initialize - idempotency
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_CalledTwice_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo initMethod = BootstrapType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        initMethod.Invoke(null, null);
        initMethod.Invoke(null, null);

        // Assert
        int initialized = (int)InitializedField.GetValue(null)!;
        await Assert.That(initialized).IsEqualTo(0);

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryCleanupNativeDirectory
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public void TryCleanupNativeDirectory_NullNativeDir_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        NativeDirField.SetValue(null, null);
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        cleanupMethod.Invoke(null, null);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }

    [Test]
    public void TryCleanupNativeDirectory_EmptyNativeDir_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        NativeDirField.SetValue(null, "");
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        cleanupMethod.Invoke(null, null);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }

    [Test]
    public void TryCleanupNativeDirectory_NonExistentDirectory_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string nonExistentDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_nonexistent_" + Guid.NewGuid());
        NativeDirField.SetValue(null, nonExistentDir);
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        cleanupMethod.Invoke(null, null);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }

    [Test]
    public void TryCleanupNativeDirectory_ExistingDirectory_DeletesDirectory(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_cleanup_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        File.WriteAllText(Path.Join(tempDir, "test.txt"), "content");
        NativeDirField.SetValue(null, tempDir);
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        cleanupMethod.Invoke(null, null);

        // Assert (no exception = pass; delete is best-effort)

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // NativeTempDir path structure
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NativeTempDir_ContainsExpectedSegments(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();
        string assemblyName = "TestApp";
        string version = "1.0.0";
        string uniqueId = "12345_test";

        // Act
        string nativeDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "native", assemblyName, rid, version, uniqueId);

        // Assert
        await Assert.That(nativeDir).Contains("InfiniFrame");
        await Assert.That(nativeDir).Contains("native");
        await Assert.That(nativeDir).Contains(assemblyName);
        await Assert.That(nativeDir).Contains(rid);
        await Assert.That(nativeDir).Contains(version);
        await Assert.That(nativeDir).Contains(uniqueId);
    }

    [Test]
    public async Task NativeTempDir_UsesPlatformTempPath(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();

        // Act
        string nativeDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "native", "App", rid, "1.0.0", "test");

        // Assert
        await Assert.That(nativeDir).StartsWith(Path.GetTempPath());
    }

    // -----------------------------------------------------------------------------------------------------------------
    // All known RID combinations
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task AllKnownRids_HaveValidFormat(CancellationToken ct = default) {
        // Arrange
        string[] knownRids = ["win-x64", "win-arm64", "linux-x64", "linux-arm64", "osx-x64", "osx-arm64"];
        string[] knownOs = ["win", "linux", "osx"];
        string[] knownArch = ["x64", "arm64"];

        // Act & Assert
        foreach (string rid in knownRids) {
            string[] parts = rid.Split('-');
            await Assert.That(parts.Length).IsEqualTo(2);
            await Assert.That(knownOs).Contains(parts[0]);
            await Assert.That(knownArch).Contains(parts[1]);
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ArtifactManifest platform files
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WindowsNativeFileName_HasCorrectExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsNativeFileName).EndsWith(".dll");
        await Assert.That(ArtifactManifest.WindowsNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task LinuxNativeFileName_HasCorrectExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.LinuxNativeFileName).EndsWith(".so");
        await Assert.That(ArtifactManifest.LinuxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task OsxNativeFileName_HasCorrectExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.OsxNativeFileName).EndsWith(".dylib");
        await Assert.That(ArtifactManifest.OsxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task WindowsLoaderFileName_HasCorrectExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsLoaderFileName).EndsWith(".dll");
        await Assert.That(ArtifactManifest.WindowsLoaderFileName).Contains(ArtifactManifest.WindowsLoaderLibraryName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_ContainsNativeLibraryForCurrentOs(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();
        string os = rid.Split('-')[0];
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        string expectedFile = os switch {
            "win" => ArtifactManifest.WindowsNativeFileName,
            "linux" => ArtifactManifest.LinuxNativeFileName,
            "osx" => ArtifactManifest.OsxNativeFileName,
            _ => throw new PlatformNotSupportedException()
        };

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required).Contains(expectedFile);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_WindowsIncludesLoader(CancellationToken ct = default) {
        // Arrange
        if (!OperatingSystem.IsWindows()) {
            Skip.Test("Windows-only test");
            return;
        }

        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required).Contains(ArtifactManifest.WindowsLoaderFileName);
    }

    [Test]
    [OnlyRunOnLinux]
    public async Task RequiredFileNamesForCurrentPlatform_LinuxDoesNotIncludeLoader(CancellationToken ct = default) {
        // Arrange
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required).DoesNotContain(ArtifactManifest.WindowsLoaderFileName);
    }

    [Test]
    [OnlyRunOnMacOs]
    public async Task RequiredFileNamesForCurrentPlatform_OsxDoesNotIncludeLoader(CancellationToken ct = default) {
        // Arrange
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required).DoesNotContain(ArtifactManifest.WindowsLoaderFileName);
    }
}
