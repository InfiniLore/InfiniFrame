// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class InfiniFramePackNativeArtifactManifest {
    public const string WindowsNativeFileName = "InfiniFrame.Native.dll";
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

    internal readonly record struct NativeRidArtifact(string RidPrefix, string FileName);
}