// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
internal static class CommandLine {
    public static ParseResult Parse(string[] args) {
        if (args.Length == 0 || IsHelp(args[0])) {
            return ParseResult.Usage(0);
        }

        string command = args[0].Trim().ToLowerInvariant();

        return string.Equals(command, "publish", StringComparison.Ordinal)
            ? ParseResult.Success(ParsePublishOptions(args.Skip(1).ToArray()))
            : throw new InvalidOperationException($"Unknown command '{args[0]}'.");
    }

    public static void PrintUsage() {
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

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static PublishOptions ParsePublishOptions(string[] args) {
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
                    options.Rid = ReadValue(args, ref index, token);
                    break;
                case "--configuration":
                    options.Configuration = ReadValue(args, ref index, token);
                    break;
                case "--framework":
                    options.Framework = ReadValue(args, ref index, token);
                    break;
                case "--self-contained":
                    options.SelfContained = bool.Parse(ReadValue(args, ref index, token));
                    break;
                case "--output":
                    options.Output = ReadValue(args, ref index, token);
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

        return !string.IsNullOrWhiteSpace(options.ProjectPath)
            ? options
            : throw new InvalidOperationException("Missing project path.");
    }

    private static string ReadValue(string[] args, ref int index, string option) {
        index++;
        if (index >= args.Length) throw new InvalidOperationException($"Missing value for {option}.");

        string value = args[index];
        index++;
        return value;
    }
}
