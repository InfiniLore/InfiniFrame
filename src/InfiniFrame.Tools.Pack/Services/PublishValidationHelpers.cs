// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Buffers.Binary;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure validation logic for publish operations.
///     Extracted from <see cref="PublishValidator"/> for testability.
/// </summary>
public static class PublishValidationHelpers {

    internal const ushort ImageFileMachineAmd64 = 0x8664;
    internal const ushort ImageFileMachineArm64 = 0xAA64;

    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    /// <summary>
    ///     Validates a RID string has the expected format (os-arch).
    /// </summary>
    public static bool ValidateRidConsistency(string rid) {
        if (string.IsNullOrWhiteSpace(rid)) throw new InvalidOperationException("Runtime identifier (RID) cannot be empty.");
        if (!rid.Contains('-')) throw new InvalidOperationException($"Invalid RID format: '{rid}'. Expected format like 'win-x64', 'linux-arm64'.");

        bool isWindowsRid = rid.StartsWith("win-", StringComparison.OrdinalIgnoreCase);
        bool isLinuxRid = rid.StartsWith("linux-", StringComparison.OrdinalIgnoreCase);
        bool isOsxRid = rid.StartsWith("osx-", StringComparison.OrdinalIgnoreCase);

        if (!isWindowsRid && !isLinuxRid && !isOsxRid) throw new InvalidOperationException($"Unsupported or unknown RID: '{rid}'.");

        return true;
    }

    /// <summary>
    ///     Validates that an output path is safe to delete.
    /// </summary>
    public static bool ValidateOutputPath(string projectDirectory, string outputPath, bool forceCleanOutput) {
        string fullPath = Path.GetFullPath(outputPath);
        if (string.IsNullOrWhiteSpace(fullPath)) throw new InvalidOperationException("Cannot delete an empty path.");

        string? root = Path.GetPathRoot(fullPath);
        if (string.Equals(fullPath, root, PathComparison)) {
            throw new InvalidOperationException($"Refusing to delete root directory '{fullPath}'.");
        }

        string projectBinDirectory = Path.GetFullPath(Path.Join(projectDirectory, "bin"));
        if (IsUnderDirectory(fullPath, projectBinDirectory)) return true;

        if (!Directory.Exists(fullPath)) return true;

        if (!forceCleanOutput) {
            throw new InvalidOperationException(
                $"Refusing to delete non-default output directory '{fullPath}'. " +
                "Pass --force-clean-output to allow this."
            );
        }

        return true;
    }

    /// <summary>
    ///     Maps a Windows RID suffix to the expected PE machine architecture.
    /// </summary>
    public static ushort ExpectedPeMachineForRid(string rid) {
        if (rid.EndsWith("-x64", StringComparison.OrdinalIgnoreCase)) return ImageFileMachineAmd64;
        if (rid.EndsWith("-arm64", StringComparison.OrdinalIgnoreCase)) return ImageFileMachineArm64;
        throw new InvalidOperationException($"Unsupported Windows RID for native artifact architecture validation: {rid}");
    }

    /// <summary>
    ///     Reads the PE machine architecture from a binary file.
    /// </summary>
    public static ushort ReadPeMachineFromStream(Stream stream) {
        long length = stream.Length;
        if (length < 0x40) throw new InvalidOperationException("Not a valid PE binary: file too short.");

        Span<byte> dosHeader = stackalloc byte[64];
        stream.ReadExactly(dosHeader);

        if (dosHeader[0] != (byte)'M' || dosHeader[1] != (byte)'Z')
            throw new InvalidOperationException("Not a valid PE binary: missing MZ signature.");

        int peHeaderOffset = BinaryPrimitives.ReadInt32LittleEndian(dosHeader[0x3C..0x40]);
        if (peHeaderOffset < 0 || peHeaderOffset > length - 6)
            throw new InvalidOperationException("Not a valid PE binary: invalid PE header offset.");

        stream.Position = peHeaderOffset;
        Span<byte> pePrefixAndMachine = stackalloc byte[6];
        stream.ReadExactly(pePrefixAndMachine);

        if (pePrefixAndMachine[0] != (byte)'P' || pePrefixAndMachine[1] != (byte)'E' || pePrefixAndMachine[2] != 0 || pePrefixAndMachine[3] != 0)
            throw new InvalidOperationException("Not a valid PE binary: missing PE signature.");

        return BinaryPrimitives.ReadUInt16LittleEndian(pePrefixAndMachine[4..6]);
    }

    /// <summary>
    ///     Returns a human-readable description of a PE machine architecture.
    /// </summary>
    public static string DescribePeMachine(ushort machine) => machine switch {
        ImageFileMachineAmd64 => $"x64 (0x{machine:X4})",
        ImageFileMachineArm64 => $"arm64 (0x{machine:X4})",
        _ => $"0x{machine:X4}"
    };

    /// <summary>
    ///     Splits a RID into platform and architecture parts with mapping.
    /// </summary>
    public static (string Platform, string Architecture)? ParseRid(string rid) {
        string[] ridParts = rid.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (ridParts.Length != 2) return null;

        string platform = ridParts[0].ToLowerInvariant() switch {
            "win" => "windows",
            "linux" => "linux",
            "osx" => "osx",
            _ => string.Empty
        };
        string architecture = ridParts[1].ToLowerInvariant() switch {
            "x64" => "x64",
            "arm64" => "arm64",
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(architecture)) return null;

        return (platform, architecture);
    }

    internal static bool IsUnderDirectory(string candidatePath, string parentPath) {
        string normalizedCandidate = EnsureTrailingSeparator(Path.GetFullPath(candidatePath));
        string normalizedParent = EnsureTrailingSeparator(Path.GetFullPath(parentPath));
        return normalizedCandidate.StartsWith(normalizedParent, PathComparison);
    }

    internal static string EnsureTrailingSeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
}
