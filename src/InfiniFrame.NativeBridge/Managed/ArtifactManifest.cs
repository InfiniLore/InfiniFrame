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
    /// <summary>The logical name of the native InfiniFrame library used in P/Invoke declarations.</summary>
    public const string NativeLibraryName = "InfiniFrame.Native";
    /// <summary>The Windows filename for the native library (e.g., <c>InfiniFrame.Native.dll</c>).</summary>
    public const string WindowsNativeFileName = $"{NativeLibraryName}.dll";
    /// <summary>The logical name of the WebView2 loader library used on Windows.</summary>
    public const string WindowsLoaderLibraryName = "WebView2Loader";
    /// <summary>The Windows filename for the WebView2 loader library.</summary>
    public const string WindowsLoaderFileName = $"{WindowsLoaderLibraryName}.dll";
    /// <summary>The Linux filename for the native library (e.g., <c>InfiniFrame.Native.so</c>).</summary>
    public const string LinuxNativeFileName = $"{NativeLibraryName}.so";
    /// <summary>The macOS filename for the native library (e.g., <c>InfiniFrame.Native.dylib</c>).</summary>
    public const string OsxNativeFileName = $"{NativeLibraryName}.dylib";

    // ReSharper disable once ConvertIfStatementToReturnStatement
    /// <summary>
    ///     Returns the platform-specific filename for the native InfiniFrame library.
    /// </summary>
    /// <returns>The library filename for the current OS.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when running on an unsupported OS.</exception>
    public static string ResolveNativeLibraryFileNameForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return LinuxNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OsxNativeFileName;

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement
    /// <summary>
    ///     Returns the list of native library filenames required on the current platform.
    /// </summary>
    /// <returns>An array of filenames that must be present for the runtime to function.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when running on an unsupported OS.</exception>
    public static string[] RequiredFileNamesForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return [WindowsNativeFileName, WindowsLoaderFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return [LinuxNativeFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return [OsxNativeFileName];

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }
}
