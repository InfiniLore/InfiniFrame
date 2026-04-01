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

        string arch;
        switch (RuntimeInformation.OSArchitecture) {
            case Architecture.X64:
                arch = "x64";
                break;
            case Architecture.Arm64:
                arch = "arm64";
                break;
            case Architecture.X86:
            case Architecture.Arm:
            case Architecture.Wasm:
            case Architecture.S390x:
            case Architecture.LoongArch64:
            case Architecture.Armv6:
            case Architecture.Ppc64le:
            case Architecture.RiscV64:
            default: throw new PlatformNotSupportedException("Only x64 and arm64 are supported for auto RID resolution.");
        }
        
        // ReSharper disable thrice ConvertIfStatementToReturnStatement
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return $"win-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"linux-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"osx-{arch}";

        throw new PlatformNotSupportedException("Unsupported OS for auto RID resolution.");
    }

    public static string ResolveNativeOsDir(string rid) {
        // ReSharper disable thrice ConvertIfStatementToReturnStatement
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return "windows";
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return "linux";
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return "osx";

        throw new InvalidOperationException($"Unsupported RID for native artifact resolution: {rid}");
    }

    public static string ResolveNativePlatform(string rid) => rid.Contains("arm64", StringComparison.OrdinalIgnoreCase) 
        ? "arm64" 
        : "x64";
}
