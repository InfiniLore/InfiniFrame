using System.CommandLine;
using System.Diagnostics;

string rid = "auto";
string? framework = null;
string configuration = "Release";
bool selfContained = true;
string? output = null;
bool verbose = false;

var ridOption = new Option<string>(
    name: "--rid",
    aliases: ["-r"]);
ridOption.Description = "Runtime identifier (e.g. win-x64, linux-arm64, osx-x64). Use 'auto' to detect.";
ridOption.DefaultValueFactory = _ => "auto";

var frameworkOption = new Option<string?>(
    name: "--framework",
    aliases: ["-f"]);
frameworkOption.Description = "Target framework. Auto-detected from project if not specified.";

var configOption = new Option<string>(
    name: "--configuration",
    aliases: ["-c"]);
configOption.Description = "Build configuration.";
configOption.DefaultValueFactory = _ => "Release";

var selfContainedOption = new Option<bool>(
    name: "--self-contained");
selfContainedOption.Description = "Produce a self-contained single-file executable.";
selfContainedOption.DefaultValueFactory = _ => true;

var outputOption = new Option<string?>(
    name: "--output",
    aliases: ["-o"]);
outputOption.Description = "Output directory. Defaults to bin/<Config>/<TFM>/<RID>/publish.";

var verboseOption = new Option<bool>(
    name: "--verbose",
    aliases: ["-v"]);
verboseOption.Description = "Show detailed build output.";

var projectArg = new Argument<FileInfo>(
    name: "project");
projectArg.Description = "Path to the .csproj file.";

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
    var project = parseResult.GetValue(projectArg);
    rid = parseResult.GetValue(ridOption)!;
    framework = parseResult.GetValue(frameworkOption);
    configuration = parseResult.GetValue(configOption)!;
    selfContained = parseResult.GetValue(selfContainedOption);
    output = parseResult.GetValue(outputOption);
    verbose = parseResult.GetValue(verboseOption);

    if (project is null || !project.Exists) {
        Console.Error.WriteLine($"Project file not found: {project?.FullName ?? "(null)"}");
        return 1;
    }

    if (rid == "auto") {
        rid = DetectRid();
        Console.WriteLine($"Auto-detected RID: {rid}");
    }

    Console.WriteLine($"Publishing {project.Name} as single-file for {rid}...");

    // Just invoke the MSBuild target — it handles the two-pass logic internally
    var args = new List<string> {
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

    if (!string.IsNullOrWhiteSpace(framework)) args.AddRange(["-f", framework]);
    if (!string.IsNullOrWhiteSpace(output)) args.AddRange(["-o", output]);

    var psi = new ProcessStartInfo("dotnet") {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        StandardOutputEncoding = System.Text.Encoding.UTF8,
        StandardErrorEncoding = System.Text.Encoding.UTF8
    };

    foreach (string arg in args) psi.ArgumentList.Add(arg);

    using var process = new Process { StartInfo = psi };
    process.OutputDataReceived += (_, e) => { if (e.Data is not null) Console.WriteLine(e.Data); };
    process.ErrorDataReceived += (_, e) => { if (e.Data is not null) Console.Error.WriteLine(e.Data); };

    if (!process.Start()) {
        Console.Error.WriteLine("Failed to start dotnet publish.");
        return 1;
    }

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    await process.WaitForExitAsync(cancellationToken);

    if (process.ExitCode == 0) Console.WriteLine("Pack completed successfully.");
    return process.ExitCode;
});

var result = await rootCommand.Parse(args).InvokeAsync();
return result;

static string DetectRid() {
    string os = OperatingSystem.IsWindows() ? "win" : OperatingSystem.IsLinux() ? "linux" : OperatingSystem.IsMacOS() ? "osx" : throw new PlatformNotSupportedException("Unsupported OS.");
    string arch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch {
        System.Runtime.InteropServices.Architecture.X64 => "x64",
        System.Runtime.InteropServices.Architecture.Arm64 => "arm64",
        _ => throw new PlatformNotSupportedException($"Unsupported architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}")
    };
    return $"{os}-{arch}";
}
