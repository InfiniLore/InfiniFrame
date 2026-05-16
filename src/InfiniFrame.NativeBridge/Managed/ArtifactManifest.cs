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
    public const string WindowsNativeFileName = "InfiniFrame.Native.dll";
    public const string WindowsLoaderLibraryName = "WebView2Loader";
    public const string WindowsLoaderFileName = "WebView2Loader.dll";
    public const string LinuxNativeFileName = "InfiniFrame.Native.so";
    public const string OsxNativeFileName = "InfiniFrame.Native.dylib";

    public static readonly NativeRidArtifact[] RidArtifacts = [
        new("win-", WindowsNativeFileName),
        new("win-", WindowsLoaderFileName),
        new("linux-", LinuxNativeFileName),
        new("osx-", OsxNativeFileName)
    ];

    public static readonly string[] AllFileNames = [
        WindowsNativeFileName,
        WindowsLoaderFileName,
        LinuxNativeFileName,
        OsxNativeFileName
    ];

    // ReSharper disable once ConvertIfStatementToReturnStatement
    public static string[] RequiredFileNamesForRid(string rid) {
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return [WindowsNativeFileName, WindowsLoaderFileName];
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return [LinuxNativeFileName];
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return [OsxNativeFileName];

        throw new InvalidOperationException($"Unsupported RID for native artifact validation: {rid}");
    }

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

    // ReSharper disable twice NotAccessedPositionalProperty.Global
    public readonly record struct NativeRidArtifact(string RidPrefix, string FileName);
}
