// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class RuntimeResolver {
    public static string ResolveRid(string requestedRid) {
        if (!string.Equals(requestedRid, "auto", StringComparison.OrdinalIgnoreCase)) return requestedRid;

        string arch = RuntimeInformation.OSArchitecture switch {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Only x64 and arm64 are supported for auto RID resolution.")
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return $"win-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"linux-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"osx-{arch}";

        throw new PlatformNotSupportedException("Unsupported OS for auto RID resolution.");
    }

    public static string ResolveNativeOsDir(string rid) {
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return "windows";
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return "linux";
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return "osx";

        throw new InvalidOperationException($"Unsupported RID for native artifact resolution: {rid}");
    }

    public static string ResolveNativePlatform(string rid) {
        return rid.Contains("arm64", StringComparison.OrdinalIgnoreCase) ? "arm64" : "x64";
    }
}
