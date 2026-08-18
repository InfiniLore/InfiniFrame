// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.CommandLine;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public static async Task<int> Main(string[] args) {
        string rid;
        string? framework;
        string configuration;
        bool selfContained;
        string? output;
        bool verbose;

        var ridOption = new Option<string>(
            name: "--rid",
            aliases: ["-r"]) {
            Description = "Runtime identifier (e.g. win-x64, linux-arm64, osx-x64). Use 'auto' to detect.",
            DefaultValueFactory = _ => "auto"
        };

        var frameworkOption = new Option<string?>(
            name: "--framework",
            aliases: ["-f"]) {
            Description = "Target framework. Auto-detected from project if not specified."
        };

        var configOption = new Option<string>(
            name: "--configuration",
            aliases: ["-c"]) {
            Description = "Build configuration.",
            DefaultValueFactory = _ => "Release"
        };

        var selfContainedOption = new Option<bool>(
            name: "--self-contained") {
            Description = "Produce a self-contained single-file executable.",
            DefaultValueFactory = _ => true
        };

        var outputOption = new Option<string?>(
            name: "--output",
            aliases: ["-o"]) {
            Description = "Output directory. Defaults to bin/<Config>/<TFM>/<RID>/publish."
        };

        var verboseOption = new Option<bool>(
            name: "--verbose",
            aliases: ["-v"]) {
            Description = "Show detailed build output."
        };

        var projectArg = new Argument<FileInfo>(
            name: "project") {
            Description = "Path to the .csproj file."
        };

        var rootCommand = new RootCommand("InfiniFrame SingleFile - Package InfiniFrame applications as single-file executables") {
            projectArg,
            ridOption,
            frameworkOption,
            configOption,
            selfContainedOption,
            outputOption,
            verboseOption
        };

        rootCommand.SetAction(async (parseResult, cancellationToken) => {
            FileInfo? project = parseResult.GetValue(projectArg);
            rid = parseResult.GetValue(ridOption)!;
            framework = parseResult.GetValue(frameworkOption);
            configuration = parseResult.GetValue(configOption)!;
            selfContained = parseResult.GetValue(selfContainedOption);
            output = parseResult.GetValue(outputOption);
            verbose = parseResult.GetValue(verboseOption);

            if (project is null || !project.Exists) {
                await Console.Error.WriteLineAsync($"Project file not found: {project?.FullName ?? "(null)"}");
                return 1;
            }

            if (rid == "auto") {
                rid = DetectRid();
                Console.WriteLine($"Auto-detected RID: {rid}");
            }

            Console.WriteLine($"Publishing {project.Name} as single-file for {rid}...");

            // Just invoke the MSBuild target, it handles the two-pass logic internally
            var list = new List<string> {
                "publish",
                project.FullName,
                "-t:InfiniFrameSingleFile",
                "-r", rid,
                "-c", configuration,
                "-p:InfiniFrameSingleFileActive=true",
                "-p:InfiniFrameSingleFileRid=" + rid,
                "-p:InfiniFrameSingleFileSelfContained=" + selfContained.ToString().ToLowerInvariant(),
                verbose ? "-v:normal" : "-v:minimal"
            };

            if (!string.IsNullOrWhiteSpace(framework)) list.AddRange(["-f", framework]);
            if (!string.IsNullOrWhiteSpace(output)) list.AddRange(["-o", output]);

            var psi = new ProcessStartInfo("dotnet") {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            foreach (string arg in list) psi.ArgumentList.Add(arg);

            using var process = new Process();
            process.StartInfo = psi;
            process.OutputDataReceived += (_, e) => {
                if (e.Data is not null) Console.WriteLine(e.Data);
            };
            process.ErrorDataReceived += (_, e) => {
                if (e.Data is not null) Console.Error.WriteLine(e.Data);
            };

            if (!process.Start()) {
                await Console.Error.WriteLineAsync("Failed to start dotnet publish.");
                return 1;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0) Console.WriteLine("Pack completed successfully.");
            return process.ExitCode;
        });

        int result = await rootCommand.Parse(args).InvokeAsync();
        return result;
    }
    
    private static string DetectRid() {
        string os;
        if (OperatingSystem.IsWindows())
            os = "win";
        else if (OperatingSystem.IsLinux())
            os = "linux";
        else if (OperatingSystem.IsMacOS())
            os = "osx";
        else
            throw new PlatformNotSupportedException("Unsupported OS.");

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
            default: throw new PlatformNotSupportedException($"Unsupported architecture: {RuntimeInformation.OSArchitecture}");
        }

        return $"{os}-{arch}";
    }
}
