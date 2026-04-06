// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Shared manifest for native artifact filenames used by pack and bootstrap flows.
/// </summary>
public static class InfiniFrameNativeArtifactManifest {
    // ReSharper disable once UnusedMember.Global
    public const string NativeLibraryName = "InfiniFrame.Native";
    // ReSharper disable once UnusedMember.Global
    public const string WindowsNativeFileName = "InfiniFrame.Native.dll";
    // ReSharper disable once UnusedMember.Global
    public const string WindowsLoaderLibraryName = "WebView2Loader";
    // ReSharper disable once UnusedMember.Global
    public const string WindowsLoaderFileName = "WebView2Loader.dll";
    // ReSharper disable once UnusedMember.Global
    public const string LinuxNativeFileName = "InfiniFrame.Native.so";
    // ReSharper disable once UnusedMember.Global
    public const string OsxNativeFileName = "InfiniFrame.Native.dylib";

    // ReSharper disable once UnusedMember.Global
    public static readonly NativeRidArtifact[] RidArtifacts = [
        new("win-", WindowsNativeFileName),
        new("win-", WindowsLoaderFileName),
        new("linux-", LinuxNativeFileName),
        new("osx-", OsxNativeFileName)
    ];

    // ReSharper disable once UnusedMember.Global
    public static readonly string[] AllFileNames = [
        WindowsNativeFileName,
        WindowsLoaderFileName,
        LinuxNativeFileName,
        OsxNativeFileName
    ];

    // ReSharper disable once ConvertIfStatementToReturnStatement, UnusedMember.Global
    public static string[] RequiredFileNamesForRid(string rid) {
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return [WindowsNativeFileName, WindowsLoaderFileName];
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return [LinuxNativeFileName];
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return [OsxNativeFileName];

        throw new InvalidOperationException($"Unsupported RID for native artifact validation: {rid}");
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement, UnusedMember.Global
    public static string ResolveNativeLibraryFileNameForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return WindowsNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return LinuxNativeFileName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return OsxNativeFileName;

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement, UnusedMember.Global
    public static string[] RequiredFileNamesForCurrentPlatform() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return [WindowsNativeFileName, WindowsLoaderFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return [LinuxNativeFileName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return [OsxNativeFileName];

        throw new PlatformNotSupportedException("Unsupported OS for native bootstrap.");
    }

    // ReSharper disable twice NotAccessedPositionalProperty.Global
    public readonly record struct NativeRidArtifact(string RidPrefix, string FileName);
}
