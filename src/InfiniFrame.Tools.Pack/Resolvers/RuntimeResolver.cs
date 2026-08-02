// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.Tools.Pack.Resolvers;
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

}