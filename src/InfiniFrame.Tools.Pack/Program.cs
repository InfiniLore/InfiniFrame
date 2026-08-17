using System.CommandLine;
using System.Diagnostics;

string rid = "auto";
string? framework = null;
string configuration = "Release";
bool selfContained = true;
string? output = null;
bool verbose = false;

var ridOption = new Option<string>(
    aliases: ["--rid", "-r"],
    description: "Runtime identifier (e.g. win-x64, linux-arm64, osx-x64). Use 'auto' to detect.",
    getDefaultValue: () => "auto");

var frameworkOption = new Option<string?>(
    aliases: ["--framework", "-f"],
    description: "Target framework. Auto-detected from project if not specified.");

var configOption = new Option<string>(
    aliases: ["--configuration", "-c"],
    description: "Build configuration.",
    getDefaultValue: () => "Release");

var selfContainedOption = new Option<bool>(
    aliases: ["--self-contained"],
    description: "Produce a self-contained single-file executable.",
    getDefaultValue: () => true);

var outputOption = new Option<string?>(
    aliases: ["--output", "-o"],
    description: "Output directory. Defaults to bin/<Config>/<TFM>/<RID>/publish.");

var verboseOption = new Option<bool>(
    aliases: ["--verbose", "-v"],
    description: "Show detailed build output.");

var projectArg = new Argument<FileInfo>(
    name: "project",
    description: "Path to the .csproj file.");

var rootCommand = new RootCommand("InfiniFrame Pack - Package InfiniFrame applications as single-file executables") {
    projectArg,
    ridOption,
    frameworkOption,
    configOption,
    selfContainedOption,
    outputOption,
    verboseOption
};

rootCommand.SetHandler(async (context) => {
    var project = context.ParseResult.GetValueForArgument(projectArg);
    rid = context.ParseResult.GetValueForOption(ridOption)!;
    framework = context.ParseResult.GetValueForOption(frameworkOption);
    configuration = context.ParseResult.GetValueForOption(configOption)!;
    selfContained = context.ParseResult.GetValueForOption(selfContainedOption);
    output = context.ParseResult.GetValueForOption(outputOption);
    verbose = context.ParseResult.GetValueForOption(verboseOption);

    if (!project.Exists) {
        Console.Error.WriteLine($"Project file not found: {project.FullName}");
        context.ExitCode = 1;
        return;
    }

    if (rid == "auto") {
        rid = DetectRid();
        Console.WriteLine($"Auto-detected RID: {rid}");
    }

    var args = BuildPublishArgs(project.FullName, rid, framework, configuration, selfContained, output, verbose);

    Console.WriteLine($"Publishing {project.Name} as single-file for {rid}...");

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
        context.ExitCode = 1;
        return;
    }

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    await process.WaitForExitAsync();

    context.ExitCode = process.ExitCode;
    if (process.ExitCode == 0) Console.WriteLine("Pack completed successfully.");
});

return await rootCommand.InvokeAsync(args);

static string DetectRid() {
    string os = OperatingSystem.IsWindows() ? "win" : OperatingSystem.IsLinux() ? "linux" : OperatingSystem.IsMacOS() ? "osx" : throw new PlatformNotSupportedException("Unsupported OS.");
    string arch = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch {
        System.Runtime.InteropServices.Architecture.X64 => "x64",
        System.Runtime.InteropServices.Architecture.Arm64 => "arm64",
        _ => throw new PlatformNotSupportedException($"Unsupported architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}")
    };
    return $"{os}-{arch}";
}

static List<string> BuildPublishArgs(string projectPath, string rid, string? framework, string configuration, bool selfContained, string? output, bool verbose) {
    var args = new List<string> {
        "publish",
        projectPath,
        "-r", rid,
        "-c", configuration,
        "-p:PublishSingleFile=true",
        "-p:InfiniFramePackEnabled=true",
        "-p:EnableCompressionInSingleFile=true",
        "-p:IncludeAllContentForSelfExtract=true",
        "-p:SelfContained=" + selfContained.ToString().ToLowerInvariant(),
        "-p:DebugType=none",
        "-p:DebugSymbols=false",
        verbose ? "-v:normal" : "-v:minimal"
    };

    if (!string.IsNullOrWhiteSpace(framework)) args.AddRange(["-f", framework]);
    if (!string.IsNullOrWhiteSpace(output)) args.AddRange(["-o", output]);

    return args;
}
