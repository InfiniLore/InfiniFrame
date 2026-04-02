// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class RuntimeResolver {
    /// <summary>
    ///     Resolves the runtime identifier to use for publish.
    /// </summary>
    /// <param name="requestedRid">Requested RID, or <c>auto</c> to infer from the current OS and architecture.</param>
    /// <returns>A concrete runtime identifier.</returns>
    /// <exception cref="PlatformNotSupportedException">
    ///     Thrown when automatic RID resolution is requested on an unsupported OS or architecture.
    /// </exception>
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

    /// <summary>
    ///     Resolves the native artifact OS directory segment from a RID.
    /// </summary>
    /// <param name="rid">Runtime identifier.</param>
    /// <returns><c>windows</c>, <c>linux</c>, or <c>osx</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the RID is unsupported.</exception>
    public static string ResolveNativeOsDir(string rid) {
        // ReSharper disable thrice ConvertIfStatementToReturnStatement
        if (rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return "windows";
        if (rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase)) return "linux";
        if (rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase)) return "osx";

        throw new InvalidOperationException($"Unsupported RID for native artifact resolution: {rid}");
    }

    /// <summary>
    ///     Resolves the native build platform from a RID.
    /// </summary>
    /// <param name="rid">Runtime identifier.</param>
    /// <returns><c>arm64</c> when the RID includes <c>arm64</c>; otherwise <c>x64</c>.</returns>
    public static string ResolveNativePlatform(string rid) => rid.Contains("arm64", StringComparison.OrdinalIgnoreCase)
        ? "arm64"
        : "x64";
}
