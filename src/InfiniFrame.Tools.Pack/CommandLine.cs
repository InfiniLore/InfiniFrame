// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.Tools.Pack;
// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
internal sealed class CommandLine {
    private readonly ILogger<CommandLine> _logger;

    public CommandLine(ILogger<CommandLine> logger) {
        _logger = logger;
    }

    /// <summary>
    ///     Parses command-line arguments into a normalized <see cref="PublishOptions" /> model or a usage response.
    /// </summary>
    /// <param name="args">Raw command-line arguments.</param>
    /// <returns>A parse result that indicates whether usage should be shown or publish options are ready.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the command is unknown, required arguments are missing, or unsupported options are provided.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown when <c>--self-contained</c> receives a value that is not a valid boolean.
    /// </exception>
    public ParseResult Parse(string[] args) {
        string? firstArg = args.FirstOrDefault();
        if (args.Length == 0 || firstArg is null || IsHelp(firstArg)) return ParseResult.Usage(ExitCodes.Success);

        string command = firstArg.Trim().ToLowerInvariant();

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!command.Equals("publish", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"Unknown command '{args[0]}'.");

        string[] argsWithoutCommand = args.Skip(1).ToArray();
        if (argsWithoutCommand.Length == 0) return ParseResult.Usage(ExitCodes.Success);

        PublishOptions result = ParsePublishOptions(argsWithoutCommand);

        return ParseResult.Success(result);
    }

    /// <summary>
    ///     Prints the CLI usage text for the pack tool.
    /// </summary>
    public void PrintUsage() {
        _logger.LogInformation("InfiniFrame.Pack");
        _logger.LogInformation("Usage:");
        _logger.LogInformation("  infiniframe-pack publish <project.csproj> [options]");
        _logger.LogInformation("");
        _logger.LogInformation("Options:");
        _logger.LogInformation("  --rid <RID|auto>             Runtime identifier. Default: auto");
        _logger.LogInformation("  --configuration <Config>      Build configuration. Default: Release");
        _logger.LogInformation("  --framework <TFM>             Target framework. Default: first TFM in project");
        _logger.LogInformation("  --self-contained <true|false> Self-contained publish. Default: true");
        _logger.LogInformation("  --output <path>               Publish output directory");
        _logger.LogInformation("  --no-restore                  Skip restore");
        _logger.LogInformation("  --verbose                     Verbose publish output");
        _logger.LogInformation("  --timeout <value>             Per-process timeout (e.g. 600, 90s, 5m, 00:10:00). Default: 10m, max: 30m");
        _logger.LogInformation("  --force-clean-output          Allow deleting non-default output directories");
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
                case "--timeout":
                    options.ProcessTimeout = TimeoutParser.Parse(ReadValue(args, ref index, token));
                    break;
                case "--force-clean-output":
                    options.ForceCleanOutput = true;
                    index++;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown option '{token}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(options.ProjectPath)) throw new InvalidOperationException("Missing project path.");
        TimeoutParser.Validate(options.ProcessTimeout);
        return options;
    }

    private static string ReadValue(string[] args, ref int index, string option) {
        index++;
        if (index >= args.Length) throw new InvalidOperationException($"Missing value for {option}.");

        string value = args[index];
        index++;
        return value;
    }
}
