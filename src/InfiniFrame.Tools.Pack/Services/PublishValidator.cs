// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Buffers.Binary;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class PublishValidator {
    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    
    
    private const ushort ImageFileMachineAmd64 = 0x8664;
    private const ushort ImageFileMachineArm64 = 0xAA64;
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Runs all preflight validation checks before publish.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when any validation step fails.
    /// </exception>
    public static void PreflightValidate(
        string projectDirectory,
        string outputPath,
        string rid,
        string nativeArtifactsDir,
        bool forceCleanOutput
    ) {
        ValidateRidConsistency(rid);
        ValidateOutputPath(projectDirectory, outputPath, forceCleanOutput);
        ValidateNativeArtifacts(nativeArtifactsDir, rid);
    }
    
    internal static bool ValidateOutputPath(
        string projectDirectory,
        string outputPath,
        bool forceCleanOutput
    ) {
        string fullPath = Path.GetFullPath(outputPath);
        if (string.IsNullOrWhiteSpace(fullPath)) throw new InvalidOperationException("Cannot delete an empty path.");

        string? root = Path.GetPathRoot(fullPath);
        if (string.Equals(fullPath, root, PathComparison)) {
            throw new InvalidOperationException($"Refusing to delete root directory '{fullPath}'.");
        }

        string projectBinDirectory = Path.GetFullPath(Path.Join(projectDirectory, "bin"));
        if (IsUnderDirectory(fullPath, projectBinDirectory)) return true;

        if (!forceCleanOutput) {
            throw new InvalidOperationException(
                $"Refusing to delete non-default output directory '{fullPath}'. " +
                "Pass --force-clean-output to allow this."
            );
        }

        return true;
    }

    private static bool IsUnderDirectory(string candidatePath, string parentPath) {
        string normalizedCandidate = EnsureTrailingSeparator(Path.GetFullPath(candidatePath));
        string normalizedParent = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        return normalizedCandidate.StartsWith(normalizedParent, PathComparison);
    }

    private static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;

    public static void ValidateNativeArtifacts(
        string nativeArtifactsDir,
        string rid
    ) {
        if (!Directory.Exists(nativeArtifactsDir)) throw new InvalidOperationException($"Native artifacts directory was not found: {nativeArtifactsDir}");

        string[] requiredPaths = InfiniFramePackNativeArtifactManifest.RequiredFileNamesForRid(rid)
            .Select(file => Path.IsPathRooted(file) ? file : Path.Join(nativeArtifactsDir, file))
            .ToArray();
        
        string? missingPath = requiredPaths.FirstOrDefault(path => !File.Exists(path));
        if (missingPath is not null) {
            throw new InvalidOperationException($"Required native artifact was not found: {missingPath}");
        }

        foreach (string path in requiredPaths) {
            if (!rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase)) return;

            ushort expectedMachine = ExpectedPeMachineForRid(rid);
            ushort actualMachine = ReadPeMachine(path);
            if (actualMachine == expectedMachine) return;

            throw new InvalidOperationException(
                $"Native artifact architecture mismatch for '{path}'. " +
                $"Expected {DescribePeMachine(expectedMachine)} for RID '{rid}', found {DescribePeMachine(actualMachine)}."
            );
        }
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
    
    internal static bool ValidateRidConsistency(string rid) {
        if (string.IsNullOrWhiteSpace(rid)) throw new InvalidOperationException("Runtime identifier (RID) cannot be empty.");

        // Basic sanity check
        if (!rid.Contains('-')) throw new InvalidOperationException($"Invalid RID format: '{rid}'. Expected format like 'win-x64', 'linux-arm64'.");

        // OS expectations
        bool isWindowsRid = rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase);
        bool isLinuxRid = rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase);
        bool isOsxRid = rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase);

        if (!isWindowsRid && !isLinuxRid && !isOsxRid) throw new InvalidOperationException($"Unsupported or unknown RID: '{rid}'.");
        return true;
    }
}
