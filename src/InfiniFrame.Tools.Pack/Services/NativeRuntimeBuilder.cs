// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Buffers.Binary;
using System.Linq;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class NativeRuntimeBuilder {
    private const ushort ImageFileMachineAmd64 = 0x8664;
    private const ushort ImageFileMachineArm64 = 0xAA64;

    /// <summary>
    ///     The native runtime file names that are stripped from the final publication output after embedding.
    /// </summary>
    public static readonly string[] NativeRuntimeFiles = InfiniFramePackNativeArtifactManifest.AllFileNames;

    /// <summary>
    ///     Validates that all required native artifacts for a RID are present in the artifact directory.
    /// </summary>
    /// <param name="nativeArtifactsDir">Directory containing native build outputs.</param>
    /// <param name="rid">Runtime identifier used to determine required native files.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the artifact directory is missing, a required file is missing, or the RID is unsupported.
    /// </exception>
    public static void ValidateArtifacts(string nativeArtifactsDir, string rid) {
        if (!Directory.Exists(nativeArtifactsDir)) throw new InvalidOperationException($"Native artifacts directory was not found: {nativeArtifactsDir}");

        string[] requiredPaths = RequiredFilesForRid(rid)
            .Select(file => Path.IsPathRooted(file) ? file : Path.Join(nativeArtifactsDir, file))
            .ToArray();
        
        string? missingPath = requiredPaths.FirstOrDefault(path => !File.Exists(path));
        if (missingPath is not null) {
            throw new InvalidOperationException($"Required native artifact was not found: {missingPath}");
        }

        foreach (string path in requiredPaths) {
            ValidateArtifactArchitecture(path, rid);
        }
    }

    private static string[] RequiredFilesForRid(string rid) => InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid(rid);

    private static void ValidateArtifactArchitecture(string artifactPath, string rid) {
        if (!rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return;

        ushort expectedMachine = ExpectedPeMachineForRid(rid);
        ushort actualMachine = ReadPeMachine(artifactPath);
        if (actualMachine == expectedMachine) return;

        throw new InvalidOperationException(
            $"Native artifact architecture mismatch for '{artifactPath}'. " +
            $"Expected {DescribePeMachine(expectedMachine)} for RID '{rid}', found {DescribePeMachine(actualMachine)}."
        );
    }

    private static ushort ExpectedPeMachineForRid(string rid) {
        if (rid.EndsWith("-x64", StringComparison.OrdinalIgnoreCase)) return ImageFileMachineAmd64;
        if (rid.EndsWith("-arm64", StringComparison.OrdinalIgnoreCase)) return ImageFileMachineArm64;
        throw new InvalidOperationException($"Unsupported Windows RID for native artifact architecture validation: {rid}");
    }

    private static ushort ReadPeMachine(string path) {
        using FileStream stream = File.OpenRead(path);
        long length = stream.Length;
        if (length < 0x40) throw new InvalidOperationException($"Native artifact is not a valid PE binary: {path}");

        Span<byte> dosHeader = stackalloc byte[64];
        stream.ReadExactly(dosHeader);

        if (dosHeader[0] != (byte)'M' || dosHeader[1] != (byte)'Z') {
            throw new InvalidOperationException($"Native artifact is not a valid PE binary: {path}");
        }

        int peHeaderOffset = BinaryPrimitives.ReadInt32LittleEndian(dosHeader[0x3C..0x40]);
        if (peHeaderOffset < 0 || peHeaderOffset > length - 6) {
            throw new InvalidOperationException($"Native artifact is not a valid PE binary: {path}");
        }

        stream.Position = peHeaderOffset;
        Span<byte> pePrefixAndMachine = stackalloc byte[6];
        stream.ReadExactly(pePrefixAndMachine);

        if (pePrefixAndMachine[0] != (byte)'P' || pePrefixAndMachine[1] != (byte)'E' || pePrefixAndMachine[2] != 0 || pePrefixAndMachine[3] != 0) {
            throw new InvalidOperationException($"Native artifact is not a valid PE binary: {path}");
        }

        return BinaryPrimitives.ReadUInt16LittleEndian(pePrefixAndMachine[4..6]);
    }

    private static string DescribePeMachine(ushort machine) => machine switch {
        ImageFileMachineAmd64 => $"x64 (0x{machine:X4})",
        ImageFileMachineArm64 => $"arm64 (0x{machine:X4})",
        _ => $"0x{machine:X4}"
    };
}
