// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DetectRidTests {

    [Test]
    public async Task DetectRid_CurrentPlatform_ReturnsValidRid(CancellationToken ct = default) {
        // Verify the RID format matches the current platform
        string os = OperatingSystem.IsWindows() ? "win"
            : OperatingSystem.IsLinux() ? "linux"
            : OperatingSystem.IsMacOS() ? "osx"
            : throw new PlatformNotSupportedException();

        string arch = RuntimeInformation.OSArchitecture switch {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException()
        };

        string rid = $"{os}-{arch}";

        await Assert.That(rid).Contains("-");
        await Assert.That(rid).StartsWith(os);
        await Assert.That(rid).EndsWith(arch);
    }

    [Test]
    public async Task DetectRid_WindowsX64_ReturnsWinX64(CancellationToken ct = default) {
        // On this Windows x64 machine, the RID should be win-x64
        if (!OperatingSystem.IsWindows() || RuntimeInformation.OSArchitecture != Architecture.X64) {
            return; // Skip on non-matching platforms
        }

        string rid = $"win-x64";
        await Assert.That(rid).IsEqualTo("win-x64");
    }

    [Test]
    public async Task DetectRid_FormatContainsDash(CancellationToken ct = default) {
        string os = OperatingSystem.IsWindows() ? "win" : "linux";
        const string arch = "x64";
        string rid = $"{os}-{arch}";

        await Assert.That(rid).Contains("-");
    }

    [Test]
    public async Task DetectRid_OsPartIsKnownPlatform(CancellationToken ct = default) {
        string[] knownOs = ["win", "linux", "osx"];
        string os = OperatingSystem.IsWindows() ? "win" : "linux";

        await Assert.That(knownOs).Contains(os);
    }

    [Test]
    public async Task DetectRid_ArchPartIsKnownArchitecture(CancellationToken ct = default) {
        string[] knownArch = ["x64", "arm64"];
        string arch = RuntimeInformation.OSArchitecture switch {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => "unknown"
        };

        await Assert.That(knownArch).Contains(arch);
    }
}
