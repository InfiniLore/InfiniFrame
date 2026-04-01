// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
internal static class CommandLine {
    public static ParseResult Parse(string[] args) {
        string? firstArg = args.FirstOrDefault();
        if (args.Length == 0 || firstArg is null || IsHelp(firstArg)) return ParseResult.Usage(0);

        string command = firstArg.Trim().ToLowerInvariant();

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!command.StartsWith("publish", StringComparison.Ordinal)) throw new InvalidOperationException($"Unknown command '{args[0]}'.");
        
        string[] argsWithoutCommand = args.Skip(1).ToArray();
        if (argsWithoutCommand.Length == 0) return ParseResult.Usage(0);
        
        PublishOptions result = ParsePublishOptions(argsWithoutCommand);
        
        return ParseResult.Success(result);
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
        var options = new PublishOptions {
            ProjectPath = string.Empty,
            Rid = "auto",
            Configuration = "Release",
            SelfContained = true
        };

        int index = 0;
        while (index < args.Length) {
            string token = args[index];
            if (!token.StartsWith('-')) {
                if (!string.IsNullOrWhiteSpace(options.ProjectPath)) throw new InvalidOperationException($"Unexpected argument '{token}'.");

                options.ProjectPath = token;
                index++;
                continue;

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
