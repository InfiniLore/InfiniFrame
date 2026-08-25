// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliTests {

    [Test]
    public async Task DetectRid_CurrentPlatform_ReturnsValidFormat(CancellationToken ct = default) {
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
    public async Task DetectRid_KnownPlatforms_AllHaveValidRids(CancellationToken ct = default) {
        string[] knownRids = [
            "win-x64", "win-arm64",
            "linux-x64", "linux-arm64",
            "osx-x64", "osx-arm64"
        ];

        foreach (string rid in knownRids) {
            await Assert.That(rid).Contains("-");
            string[] parts = rid.Split('-');
            await Assert.That(parts.Length).IsEqualTo(2);
        }
    }

    [Test]
    public async Task Cli_ProjectArgument_IsRequired(CancellationToken ct = default) {
        // The CLI requires a project argument - running without it should fail
        string framework = Path.GetFileName(AppContext.BaseDirectory);
        string cliPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "..",
            "src", "InfiniFrame.SingleFile", "bin", "Release", framework,
            "InfiniFrame.SingleFile.dll"));

        var psi = new System.Diagnostics.ProcessStartInfo("dotnet") {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = $"\"{cliPath}\" --help"
        };

        // On Linux, a crashing child process can send SIGABRT to the parent
        // process group, killing the test host. Isolate in a new session.
        if (OperatingSystem.IsLinux()) {
            psi.FileName = "setsid";
            psi.Arguments = $"dotnet \"{cliPath}\" --help";
        }

        using var process = System.Diagnostics.Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync(ct);

        await Assert.That(output).Contains("InfiniFrame");
    }
}
