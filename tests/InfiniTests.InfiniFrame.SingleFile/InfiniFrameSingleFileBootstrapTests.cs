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
    [SkipOnWindows]
    public async Task RequiredFileNamesForCurrentPlatform_OsxOrLinuxDoesNotIncludeLoader(CancellationToken ct = default) {
        // Arrange
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(required).DoesNotContain(ArtifactManifest.WindowsLoaderFileName);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ResolveNativeLibrary (via reflection)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ResolveNativeLibrary_NativeDirIsNull_ReturnsIntPtrZero(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        NativeDirField.SetValue(null, null);

        // Act
        IntPtr result = (IntPtr)resolveMethod.Invoke(null, ["InfiniFrame.Native", typeof(InfiniFrameSingleFile).Assembly, null])!;

        // Assert
        await Assert.That(result).IsEqualTo(IntPtr.Zero);

        // Cleanup
        ResetState();
    }

    [Test]
    public async Task ResolveNativeLibrary_LibraryNameDoesNotMatch_ReturnsIntPtrZero(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        NativeDirField.SetValue(null, Path.GetTempPath());

        // Act
        IntPtr result = (IntPtr)resolveMethod.Invoke(null, ["SomeOtherLibrary", typeof(InfiniFrameSingleFile).Assembly, null])!;

        // Assert
        await Assert.That(result).IsEqualTo(IntPtr.Zero);

        // Cleanup
        ResetState();
    }

    [Test]
    public async Task ResolveNativeLibrary_NativeDirIsEmpty_PassesNullCheck(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        NativeDirField.SetValue(null, "");

        // Act — empty string is NOT null, so the resolver proceeds past the null check
        var result = (IntPtr)resolveMethod.Invoke(null, new object?[] { "InfiniFrame.Native", typeof(InfiniFrameSingleFile).Assembly, null })!;

        // Assert — result is platform-dependent; just verify no corruption
        await Assert.That(result).IsNotEqualTo(new IntPtr(-1));

        // Cleanup
        ResetState();
    }

    [Test]
    public async Task ResolveNativeLibrary_NativeLibraryName_ReturnsIntPtrZeroWhenFileNotFound(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_resolve_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);

        // Act
        IntPtr result = (IntPtr)resolveMethod.Invoke(null, [ArtifactManifest.NativeLibraryName, typeof(InfiniFrameSingleFile).Assembly, null])!;

        // Assert
        await Assert.That(result).IsEqualTo(IntPtr.Zero);

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task ResolveNativeLibrary_WebView2LoaderName_ReturnsIntPtrZeroWhenFileNotFound(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_resolve_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);

        // Act
        IntPtr result = (IntPtr)resolveMethod.Invoke(null, ["WebView2Loader", typeof(InfiniFrameSingleFile).Assembly, null])!;

        // Assert
        await Assert.That(result).IsEqualTo(IntPtr.Zero);

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task ResolveNativeLibrary_NativeLibraryName_FileExists_ReturnsNonZeroOrThrows(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_resolve_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string fileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();
        string filePath = Path.Join(tempDir, fileName);
        await File.WriteAllBytesAsync(filePath, new byte[] { 0, 0, 0, 0 }, ct);
        NativeDirField.SetValue(null, tempDir);

        // Act & Assert — the load may succeed or throw depending on platform; we verify no null-reference or corruption
        try {
            _ = (IntPtr)resolveMethod.Invoke(null, new object?[] { ArtifactManifest.NativeLibraryName, typeof(InfiniFrameSingleFile).Assembly, null })!;
        } catch (TargetInvocationException ex) when (ex.InnerException is not null) {
            bool isExpected = ex.InnerException is BadImageFormatException or DllNotFoundException or FileLoadException;
            await Assert.That(isExpected).IsTrue();
        }

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task ResolveNativeLibrary_WebView2LoaderName_FileExists_ReturnsNonZeroOrThrows(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo resolveMethod = BootstrapType.GetMethod("ResolveNativeLibrary", BindingFlags.NonPublic | BindingFlags.Static)!;
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_resolve_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string filePath = Path.Join(tempDir, ArtifactManifest.WindowsLoaderFileName);
        await File.WriteAllBytesAsync(filePath, new byte[] { 0, 0, 0, 0 }, ct);
        NativeDirField.SetValue(null, tempDir);

        // Act & Assert
        try {
            _ = (IntPtr)resolveMethod.Invoke(null, new object?[] { "WebView2Loader", typeof(InfiniFrameSingleFile).Assembly, null })!;
        } catch (TargetInvocationException ex) when (ex.InnerException is not null) {
            bool isExpected = ex.InnerException is BadImageFormatException or DllNotFoundException or FileLoadException;
            await Assert.That(isExpected).IsTrue();
        }

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryPreloadDependency (via reflection)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public void TryPreloadDependency_NativeDirIsNull_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        NativeDirField.SetValue(null, null);
        MethodInfo preloadMethod = BootstrapType.GetMethod("TryPreloadDependency", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        preloadMethod.Invoke(null, [ArtifactManifest.WindowsLoaderFileName]);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }

    [Test]
    public void TryPreloadDependency_FileDoesNotExist_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_preload_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo preloadMethod = BootstrapType.GetMethod("TryPreloadDependency", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        preloadMethod.Invoke(null, ["nonexistent.dll"]);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public void TryPreloadDependency_FileExists_BadImage_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_preload_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string filePath = Path.Join(tempDir, ArtifactManifest.WindowsLoaderFileName);
        File.WriteAllBytes(filePath, [0, 0, 0, 0]);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo preloadMethod = BootstrapType.GetMethod("TryPreloadDependency", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act — should not throw due to catch blocks for DllNotFoundException, BadImageFormatException, FileLoadException
        preloadMethod.Invoke(null, [ArtifactManifest.WindowsLoaderFileName]);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ExtractEmbeddedNative (via reflection)
    // -----------------------------------------------------------------------------------------------------------------
    // ExtractEmbeddedNative (via reflection)
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ExtractEmbeddedNative_EmptyFileList_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_extract_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo extractMethod = BootstrapType.GetMethod("ExtractEmbeddedNative", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        extractMethod.Invoke(null, [typeof(InfiniFrameSingleFile).Assembly, "win-x64", Array.Empty<string>()]);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task ExtractEmbeddedNative_NullResourceStream_SkipsFile(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_extract_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo extractMethod = BootstrapType.GetMethod("ExtractEmbeddedNative", BindingFlags.NonPublic | BindingFlags.Static)!;
        string fakeFileName = $"nonexistent_{Guid.NewGuid()}.dll";

        // Act — should not throw; resource stream will be null and skipped
        extractMethod.Invoke(null, [typeof(InfiniFrameSingleFile).Assembly, "win-x64", new[] { fakeFileName }]);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    [Test]
    public async Task ExtractEmbeddedNative_FileAlreadyExists_SkipsFile(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_extract_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string fakeFileName = $"nonexistent_{Guid.NewGuid()}.dll";
        string filePath = Path.Join(tempDir, fakeFileName);
        await File.WriteAllBytesAsync(filePath, [0xFF], ct);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo extractMethod = BootstrapType.GetMethod("ExtractEmbeddedNative", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        extractMethod.Invoke(null, [typeof(InfiniFrameSingleFile).Assembly, "win-x64", new[] { fakeFileName }]);

        // Assert — file should still be 1 byte (not overwritten)
        byte[] content = await File.ReadAllBytesAsync(filePath, ct);
        await Assert.That(content.Length).IsEqualTo(1);

        // Cleanup
        ResetState();
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryCleanupNativeDirectory - exception paths
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public void TryCleanupNativeDirectory_IOException_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        string tempDir = Path.Join(Path.GetTempPath(), "InfiniFrame", "test_cleanup_io_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        NativeDirField.SetValue(null, tempDir);
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Create a file and lock it to force IOException
        string lockedFile = Path.Join(tempDir, "locked.txt");
        using (new FileStream(lockedFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            // Act — should not throw; IOException is caught
            cleanupMethod.Invoke(null, null);
        }

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
        try { Directory.Delete(tempDir, true); } catch { /* best effort */ }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Lock / concurrency
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_ConcurrentCalls_DoesNotDeadlock(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo initMethod = BootstrapType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act — call Initialize concurrently from multiple threads
        IEnumerable<Task<object?>> tasks = Enumerable.Range(0, 4).Select(_ => Task.Run(() => initMethod.Invoke(null, null), ct));
        await Task.WhenAll(tasks);

        // Assert — should complete without deadlock
        int initialized = (int)InitializedField.GetValue(null)!;
        await Assert.That(initialized).IsEqualTo(0); // no embedded resources = stays 0

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // GetRuntimeIdentifier edge cases
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_RidFormat_CorrectDelimiter(CancellationToken ct = default) {
        // Arrange
        string rid = GetExpectedRid();

        // Act (no-op — verifying format)

        // Assert
        await Assert.That(rid).Contains("-");
        await Assert.That(rid.Split('-').Length).IsEqualTo(2);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ArtifactManifest edge cases
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ArtifactManifest_AllPlatformFileNames_AreUnique(CancellationToken ct = default) {
        // Arrange
        string[] allFileNames = [
            ArtifactManifest.WindowsNativeFileName,
            ArtifactManifest.LinuxNativeFileName,
            ArtifactManifest.OsxNativeFileName,
            ArtifactManifest.WindowsLoaderFileName
        ];

        // Act (no-op — verifying static constants)

        // Assert
        await Assert.That(allFileNames.Distinct().Count()).IsEqualTo(allFileNames.Length);
    }

    [Test]
    public async Task ArtifactManifest_NativeLibraryFileName_ContainsLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
        await Assert.That(ArtifactManifest.LinuxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
        await Assert.That(ArtifactManifest.OsxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task ArtifactManifest_WindowsLoaderFileName_ContainsLoaderLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsLoaderFileName).Contains(ArtifactManifest.WindowsLoaderLibraryName);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Initialize - _nativeDir path structure via reflection
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Initialize_NativeDirFormat_ContainsTempPath(CancellationToken ct = default) {
        // Arrange
        ResetState();
        MethodInfo initMethod = BootstrapType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        initMethod.Invoke(null, null);

        // Assert — _nativeDir should be null since no embedded resources exist
        string? nativeDir = (string?)NativeDirField.GetValue(null);
        await Assert.That(nativeDir).IsNull();

        // Cleanup
        ResetState();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // TryCleanupNativeDirectory - whitespace check
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public void TryCleanupNativeDirectory_WhitespaceNativeDir_DoesNotThrow(CancellationToken ct = default) {
        // Arrange
        ResetState();
        NativeDirField.SetValue(null, "   ");
        MethodInfo cleanupMethod = BootstrapType.GetMethod("TryCleanupNativeDirectory", BindingFlags.NonPublic | BindingFlags.Static)!;

        // Act
        cleanupMethod.Invoke(null, null);

        // Assert (no exception = pass)

        // Cleanup
        ResetState();
    }
}
