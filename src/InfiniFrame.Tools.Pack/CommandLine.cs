// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;

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
                    options.ProcessTimeout = ParseTimeout(ReadValue(args, ref index, token));
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
        ValidateProcessTimeout(options.ProcessTimeout);
        return options;
    }

    private static string ReadValue(string[] args, ref int index, string option) {
        index++;
        if (index >= args.Length) throw new InvalidOperationException($"Missing value for {option}.");

        string value = args[index];
        index++;
        return value;
    }

    private static TimeSpan ParseTimeout(string value) {
        if (string.IsNullOrWhiteSpace(value)) throw new FormatException("Timeout value cannot be empty.");

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds) && seconds > 0) {
            return TimeSpan.FromSeconds(seconds);
        }

        if (TryParseUnitTimeout(value, out TimeSpan unitTimeout)) return unitTimeout;
        if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out TimeSpan timeSpan) && timeSpan > TimeSpan.Zero) return timeSpan;

        throw new FormatException($"Invalid timeout value '{value}'. Use a positive value like '600', '90s', '5m', or '00:10:00'.");
    }

    private static bool TryParseUnitTimeout(string value, out TimeSpan timeout) {
        timeout = default;
        if (value.Length < 2) return false;

        char unit = char.ToLowerInvariant(value[^1]);
        string numberPart = value[..^1];
        if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out double quantity) || quantity <= 0) {
            return false;
        }

        timeout = unit switch {
            's' => TimeSpan.FromSeconds(quantity),
            'm' => TimeSpan.FromMinutes(quantity),
            'h' => TimeSpan.FromHours(quantity),
            _ => default
        };

        return timeout > TimeSpan.Zero;
    }

    private static void ValidateProcessTimeout(TimeSpan timeout) {
        if (timeout <= TimeSpan.Zero) {
            throw new FormatException($"Timeout must be greater than zero. Received '{timeout}'.");
        }

        if (timeout > PublishOptions.MaxProcessTimeout) {
            throw new FormatException(
                $"Timeout '{timeout}' exceeds the maximum supported value of '{PublishOptions.MaxProcessTimeout}'.");
        }
    }
}
