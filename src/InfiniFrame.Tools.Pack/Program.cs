// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public static async Task<int> Main(string[] args) {
        try {
            if (args.Length == 0 || IsHelp(args[0])) {
                PrintUsage();
                return 0;
            }

            string command = args[0].Trim().ToLowerInvariant();
            if (command != "publish") {
                await Console.Error.WriteLineAsync($"Unknown command '{args[0]}'.");
                PrintUsage();
                return 1;
            }

            PublishOptions options = ParsePublishArgs(args.Skip(1).ToArray());
            return await PublishAsync(options);
        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"[InfiniFrame.Pack] ERROR: {ex.Message}");
            return 1;
        }
    }

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static void PrintUsage() {
        Console.WriteLine("InfiniFrame.Pack");
        Console.WriteLine("Usage:");
        Console.WriteLine("  infiniframe-pack publish <project.csproj> [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --rid <RID|auto>             Runtime identifier. Default: auto");
        Console.WriteLine("  --configuration <Config>      Build configuration. Default: Release");
        Console.WriteLine("  --framework <TFM>             Target framework. Default: first TFM in project");
        Console.WriteLine("  --self-contained <true|false> Self-contained publish. Default: true");
        Console.WriteLine("  --output <path>               Publish output directory");
        Console.WriteLine("  --no-restore                  Skip restore");
        Console.WriteLine("  --verbose                     Verbose publish output");
    }

    private static PublishOptions ParsePublishArgs(string[] args) {
        if (args.Length == 0) throw new InvalidOperationException("Missing project path.");

        var options = new PublishOptions {
            ProjectPath = string.Empty,
            Rid = "auto",
            Configuration = "Release",
            SelfContained = true
        };

        int index = 0;
        while (index < args.Length) {
            string token = args[index];
            if (!token.StartsWith("-", StringComparison.Ordinal)) {
                if (string.IsNullOrWhiteSpace(options.ProjectPath)) {
                    options.ProjectPath = token;
                    index++;
                    continue;
                }

                throw new InvalidOperationException($"Unexpected argument '{token}'.");
            }

            switch (token) {
                case "--rid":
                    options.Rid = RequireValue(args, ref index, token);
                    break;
                case "--configuration":
                    options.Configuration = RequireValue(args, ref index, token);
                    break;
                case "--framework":
                    options.Framework = RequireValue(args, ref index, token);
                    break;
                case "--self-contained":
                    options.SelfContained = bool.Parse(RequireValue(args, ref index, token));
                    break;
                case "--output":
                    options.Output = RequireValue(args, ref index, token);
                    break;
                case "--no-restore":
                    options.NoRestore = true;
                    index++;
                    break;
                case "--verbose":
                    options.Verbose = true;
                    index++;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown option '{token}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(options.ProjectPath)) {
            throw new InvalidOperationException("Missing project path.");
        }

        return options;
    }

    private static string RequireValue(string[] args, ref int index, string option) {
        index++;
        if (index >= args.Length) throw new InvalidOperationException($"Missing value for {option}.");
        string value = args[index];
        index++;
        return value;
    }

    private static async Task<int> PublishAsync(PublishOptions options) {
        string projectPath = Path.GetFullPath(options.ProjectPath);
        if (!File.Exists(projectPath)) throw new FileNotFoundException("Project file not found", projectPath);

        string projectDirectory = Path.GetDirectoryName(projectPath) ?? throw new InvalidOperationException("Unable to resolve project directory.");
        string framework = string.IsNullOrWhiteSpace(options.Framework) ? ResolveFramework(projectPath) : options.Framework!;
        string rid = ResolveRid(options.Rid);
        string output = string.IsNullOrWhiteSpace(options.Output)
            ? Path.Combine(projectDirectory, "bin", options.Configuration, framework, rid, "publish")
            : Path.GetFullPath(options.Output!);

        string assemblyName = ResolveAssemblyName(projectPath);

        Console.WriteLine("[InfiniFrame.Pack] Publishing single-file app");
        Console.WriteLine($"  Project: {projectPath}");
        Console.WriteLine($"  Framework: {framework}");
        Console.WriteLine($"  RID: {rid}");
        Console.WriteLine($"  SelfContained: {options.SelfContained}");
        Console.WriteLine($"  Output: {output}");

        if (Directory.Exists(output)) Directory.Delete(output, recursive: true);
        Directory.CreateDirectory(output);

        string tempTargets = CreateTempTargetsFile();
        try {
            var publishArgs = new List<string> {
                "publish",
                projectPath,
                "-c", options.Configuration,
                "-r", rid,
                "-f", framework,
                "--output", output,
                "-p:PublishSingleFile=true",
                $"-p:SelfContained={options.SelfContained.ToString().ToLowerInvariant()}",
                "-p:IncludeNativeLibrariesForSelfExtract=true",
                "-p:IncludeAllContentForSelfExtract=true",
                "-p:EnableCompressionInSingleFile=true",
                "-p:DebugType=none",
                "-p:DebugSymbols=false",
                $"-p:InfiniFramePackRootProject={projectPath}",
                $"-p:CustomAfterMicrosoftCommonTargets={tempTargets}",
                options.Verbose ? "-v:normal" : "-v:minimal"
            };

            if (options.NoRestore) publishArgs.Add("--no-restore");

            int exitCode = await RunProcessAsync("dotnet", publishArgs);
            if (exitCode != 0) return exitCode;

            CleanupPublishDirectory(output);

            string expectedMain = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(output, $"{assemblyName}.exe")
                : Path.Combine(output, assemblyName);

            if (!File.Exists(expectedMain)) {
                Console.WriteLine("[InfiniFrame.Pack] Publish succeeded, but expected single-file output was not found.");
            }

            string[] files = Directory.GetFiles(output, "*", SearchOption.TopDirectoryOnly);
            Console.WriteLine("[InfiniFrame.Pack] Completed");
            Console.WriteLine($"  Files in output: {files.Length}");
            foreach (string file in files.Select(Path.GetFileName).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x)!) {
                Console.WriteLine($"  - {file}");
            }

            return 0;
        }
        finally {
            TryDelete(tempTargets);
        }
    }

    private static string ResolveFramework(string projectPath) {
        XDocument doc = XDocument.Load(projectPath);
        XElement root = doc.Root ?? throw new InvalidOperationException("Invalid project file.");

        string? targetFramework = root.Descendants().FirstOrDefault(x => x.Name.LocalName == "TargetFramework")?.Value?.Trim();
        if (!string.IsNullOrWhiteSpace(targetFramework)) return targetFramework;

        string? targetFrameworks = root.Descendants().FirstOrDefault(x => x.Name.LocalName == "TargetFrameworks")?.Value?.Trim();
        return string.IsNullOrWhiteSpace(targetFrameworks) ? throw new InvalidOperationException("Could not resolve target framework from project file. Use --framework.") : targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();

    }

    private static string ResolveAssemblyName(string projectPath) {
        XDocument doc = XDocument.Load(projectPath);
        XElement root = doc.Root ?? throw new InvalidOperationException("Invalid project file.");
        string? assemblyName = root.Descendants().FirstOrDefault(x => x.Name.LocalName == "AssemblyName")?.Value?.Trim();
        if (!string.IsNullOrWhiteSpace(assemblyName)) return assemblyName;
        return Path.GetFileNameWithoutExtension(projectPath);
    }

    private static string ResolveRid(string requestedRid) {
        if (!string.Equals(requestedRid, "auto", StringComparison.OrdinalIgnoreCase)) return requestedRid;

        string arch = RuntimeInformation.OSArchitecture switch {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Only x64 and arm64 are supported for auto RID resolution.")
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return $"win-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return $"linux-{arch}";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return $"osx-{arch}";

        throw new PlatformNotSupportedException("Unsupported OS for auto RID resolution.");
    }

    private static string CreateTempTargetsFile() {
        string path = Path.Combine(Path.GetTempPath(), $"infiniframe-pack-{Guid.NewGuid():N}.targets");
        string content = """
<Project>
  <ItemGroup Condition="'$(MSBuildProjectFullPath)' == '$(InfiniFramePackRootProject)' and Exists('$(MSBuildProjectDirectory)\\wwwroot')">
    <_InfiniFramePackWwwroot Include="wwwroot\\**\\*" />
    <_InfiniFramePackWwwroot Remove="@(EmbeddedResource)" />
    <EmbeddedResource Include="@(_InfiniFramePackWwwroot)"
                      LogicalName="$(AssemblyName).wwwroot.%(RecursiveDir)%(Filename)%(Extension)" />
    <Content Remove="wwwroot\\**\\*" />
    <None Remove="wwwroot\\**\\*" />
  </ItemGroup>
</Project>
""";

        File.WriteAllText(path, content);
        return path;
    }

    private static void CleanupPublishDirectory(string output) {
        string wwwroot = Path.Combine(output, "wwwroot");
        if (Directory.Exists(wwwroot)) Directory.Delete(wwwroot, recursive: true);

        string[] toDelete = [
            "InfiniFrame.Native.dll",
            "WebView2Loader.dll",
            "InfiniFrame.Native.so",
            "InfiniFrame.Native.dylib"
        ];

        foreach (string file in toDelete) {
            string fullPath = Path.Combine(output, file);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }

    private static void TryDelete(string path) {
        try {
            if (File.Exists(path)) File.Delete(path);
        }
        catch {
            // no-op
        }
    }

    private static async Task<int> RunProcessAsync(string fileName, IReadOnlyList<string> arguments) {
        var psi = new ProcessStartInfo(fileName) {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string arg in arguments) psi.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        process.OutputDataReceived += (_, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) => {
            if (!string.IsNullOrWhiteSpace(e.Data)) Console.Error.WriteLine(e.Data);
        };

        if (!process.Start()) throw new InvalidOperationException($"Failed to start process: {fileName}");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        return process.ExitCode;
    }

    private sealed class PublishOptions {
        public required string ProjectPath { get; set; }
        public required string Rid { get; set; }
        public required string Configuration { get; set; }
        public string? Framework { get; set; }
        public required bool SelfContained { get; set; }
        public string? Output { get; set; }
        public bool NoRestore { get; set; }
        public bool Verbose { get; set; }
    }
}
