// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using InfiniFrame.NativeBridge;
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("Usage", "TUnitAssertions0005:Assert.That(...) should not be used with a constant value")]
public class ArtifactManifestTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Constants, exact values
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task NativeLibraryName_IsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act
        const string name = ArtifactManifest.NativeLibraryName;

        // Assert
        await Assert.That(name).IsEqualTo("InfiniFrame.Native");
    }

    [Test]
    public async Task WindowsNativeFileName_IsNativeLibraryNameWithDllExtension(CancellationToken ct = default) {
        // Arrange & Act
        const string fileName = ArtifactManifest.WindowsNativeFileName;

        // Assert
        await Assert.That(fileName).IsEqualTo("InfiniFrame.Native.dll");
    }

    [Test]
    public async Task WindowsLoaderLibraryName_IsExpectedValue(CancellationToken ct = default) {
        // Arrange & Act
        const string name = ArtifactManifest.WindowsLoaderLibraryName;

        // Assert
        await Assert.That(name).IsEqualTo("WebView2Loader");
    }

    [Test]
    public async Task WindowsLoaderFileName_IsLoaderLibraryNameWithDllExtension(CancellationToken ct = default) {
        // Arrange & Act
        const string fileName = ArtifactManifest.WindowsLoaderFileName;

        // Assert
        await Assert.That(fileName).IsEqualTo("WebView2Loader.dll");
    }

    [Test]
    public async Task LinuxNativeFileName_IsNativeLibraryNameWithSoExtension(CancellationToken ct = default) {
        // Arrange & Act
        const string fileName = ArtifactManifest.LinuxNativeFileName;

        // Assert
        await Assert.That(fileName).IsEqualTo("InfiniFrame.Native.so");
    }

    [Test]
    public async Task OsxNativeFileName_IsNativeLibraryNameWithDylibExtension(CancellationToken ct = default) {
        // Arrange & Act
        const string fileName = ArtifactManifest.OsxNativeFileName;

        // Assert
        await Assert.That(fileName).IsEqualTo("InfiniFrame.Native.dylib");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Constants, structural invariants
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task WindowsNativeFileName_ContainsNativeLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task WindowsNativeFileName_HasDllExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsNativeFileName).EndsWith(".dll");
    }

    [Test]
    public async Task LinuxNativeFileName_ContainsNativeLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.LinuxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task LinuxNativeFileName_HasSoExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.LinuxNativeFileName).EndsWith(".so");
    }

    [Test]
    public async Task OsxNativeFileName_ContainsNativeLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.OsxNativeFileName).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task OsxNativeFileName_HasDylibExtension(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.OsxNativeFileName).EndsWith(".dylib");
    }

    [Test]
    public async Task WindowsLoaderFileName_ContainsLoaderLibraryName(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(ArtifactManifest.WindowsLoaderFileName).Contains(ArtifactManifest.WindowsLoaderLibraryName);
    }

    [Test]
    public async Task AllNativeFileNames_AreDistinct(CancellationToken ct = default) {
        // Arrange
        string[] allNames = [
            ArtifactManifest.WindowsNativeFileName,
            ArtifactManifest.LinuxNativeFileName,
            ArtifactManifest.OsxNativeFileName,
            ArtifactManifest.WindowsLoaderFileName
        ];

        // Act
        int distinctCount = allNames.Distinct().Count();

        // Assert
        await Assert.That(distinctCount).IsEqualTo(allNames.Length);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ResolveNativeLibraryFileNameForCurrentPlatform
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ResolveNativeLibraryFileNameForCurrentPlatform_ReturnsNonEmptyString(CancellationToken ct = default) {
        // Act
        string result = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Assert
        await Assert.That(result).IsNotEmpty();
    }

    [Test]
    public async Task ResolveNativeLibraryFileNameForCurrentPlatform_ContainsNativeLibraryName(CancellationToken ct = default) {
        // Act
        string result = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Assert, the native library base name must always appear in the file name
        await Assert.That(result).Contains(ArtifactManifest.NativeLibraryName);
    }

    [Test]
    public async Task ResolveNativeLibraryFileNameForCurrentPlatform_ReturnsExpectedFilenameForCurrentPlatform(CancellationToken ct = default) {
        // Arrange
        string expected;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            expected = ArtifactManifest.WindowsNativeFileName;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            expected = ArtifactManifest.LinuxNativeFileName;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            expected = ArtifactManifest.OsxNativeFileName;
        else {
            Skip.Test("Unsupported platform, skip without failing");
            return;
        }

        // Act
        string actual = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    public async Task ResolveNativeLibraryFileNameForCurrentPlatform_HasExpectedExtensionForCurrentPlatform(CancellationToken ct = default) {
        // Arrange
        string expectedExtension;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            expectedExtension = ".dll";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            expectedExtension = ".so";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            expectedExtension = ".dylib";
        else {
            Skip.Test("Unsupported platform, skip without failing");
            return;
        }

        // Act
        string result = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Assert
        await Assert.That(result).EndsWith(expectedExtension);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // RequiredFileNamesForCurrentPlatform
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_ReturnsNonEmptyArray(CancellationToken ct = default) {
        // Act
        string[] result = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert
        await Assert.That(result.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_AlwaysContainsNativeLibraryFileName(CancellationToken ct = default) {
        // Arrange
        string nativeFileName = ArtifactManifest.ResolveNativeLibraryFileNameForCurrentPlatform();

        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert, the native library must always be in the required set
        await Assert.That(required).Contains(nativeFileName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_OnWindows_ReturnsTwoFiles(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert, Windows needs the native DLL and the WebView2Loader DLL
        await Assert.That(required.Length).IsEqualTo(2);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_OnWindows_ContainsBothNativeAndLoaderFile(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert
        await Assert.That(required).Contains(ArtifactManifest.WindowsNativeFileName);
        await Assert.That(required).Contains(ArtifactManifest.WindowsLoaderFileName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_OnLinux_ReturnsOneFile(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert
        await Assert.That(required.Length).IsEqualTo(1);
        await Assert.That(required[0]).IsEqualTo(ArtifactManifest.LinuxNativeFileName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_OnOsx_ReturnsOneFile(CancellationToken ct = default) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;

        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert
        await Assert.That(required.Length).IsEqualTo(1);
        await Assert.That(required[0]).IsEqualTo(ArtifactManifest.OsxNativeFileName);
    }

    [Test]
    public async Task RequiredFileNamesForCurrentPlatform_AllEntriesAreNonEmpty(CancellationToken ct = default) {
        // Act
        string[] required = ArtifactManifest.RequiredFileNamesForCurrentPlatform();

        // Assert
        foreach (string t in required) {
            await Assert.That(t).IsNotEmpty();
        }
    }
}
