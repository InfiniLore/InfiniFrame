// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Shared manifest for native artifact filenames used by pack and bootstrap flows.
/// </summary>
public static class ArtifactManifest {
    public const string NativeLibraryName = "InfiniFrame.Native";
    public const string WindowsNativeFileName = $"{NativeLibraryName}.dll";
    public const string WindowsLoaderLibraryName = "WebView2Loader";
    public const string WindowsLoaderFileName = $"{WindowsLoaderLibraryName}.dll";
    public const string LinuxNativeFileName = $"{NativeLibraryName}.so";
    public const string OsxNativeFileName = $"{NativeLibraryName}.dylib";

    // ReSharper disable once ConvertIfStatementToReturnStatement
    public static string ResolveNativeLibraryFileNameForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return LinuxNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OsxNativeFileName;

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement
    public static string[] RequiredFileNamesForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return [WindowsNativeFileName, WindowsLoaderFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return [LinuxNativeFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return [OsxNativeFileName];

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }
}