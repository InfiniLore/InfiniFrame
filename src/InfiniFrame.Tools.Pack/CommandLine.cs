// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrame.Tools.Pack;
// -----------------------------------------------------------------------------------------------------------------
// Methods
// -----------------------------------------------------------------------------------------------------------------
internal static class CommandLine {
    private static readonly Serilog.ILogger Logger = Serilog.Log.ForContext(typeof(CommandLine));
    private const string NativeArtifactsFallbackPathEnvVar = "INFINIFRAME_PACK_NATIVE_ARTIFACTS_FALLBACK";
    private const string AllowStaleNativeFallbackEnvVar = "INFINIFRAME_PACK_ALLOW_STALE_NATIVE_FALLBACK";

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
    public static ParseResult Parse(string[] args) {
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
    public static void PrintUsage() {
        Logger.Information("InfiniFrame.Pack");
        Logger.Information("Usage:");
        Logger.Information("  infiniframe-pack publish <project.csproj> [options]");
        Logger.Information("");
        Logger.Information("Options:");
        Logger.Information("  --rid <RID|auto>             Runtime identifier. Default: auto");
        Logger.Information("  --configuration <Config>      Build configuration. Default: Release");
        Logger.Information("  --framework <TFM>             Target framework. Default: first TFM in project");
        Logger.Information("  --self-contained <true|false> Self-contained publish. Default: true");
        Logger.Information("  --output <path>               Publish output directory");
        Logger.Information("  --no-restore                  Skip restore");
        Logger.Information("  --verbose                     Verbose publish output");
        Logger.Information("  --force-clean-output          Allow deleting non-default output directories");
        Logger.Information("  --native-artifacts-fallback <path>");
        Logger.Information("                                Explicit fallback directory for native artifacts");
        Logger.Information("  --allow-stale-native-fallback");
        Logger.Information("                                Allow using fallback artifacts when preflight fails validation");
        Logger.Information("");
        Logger.Information("Environment overrides:");
        Logger.Information("  {FallbackEnvVar}=<path>", NativeArtifactsFallbackPathEnvVar);
        Logger.Information("  {AllowStaleEnvVar}=true|false", AllowStaleNativeFallbackEnvVar);
    }

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static PublishOptions ParsePublishOptions(string[] args) {
        var options = new PublishOptions {
            ProjectPath = string.Empty,
            Rid = "auto",
            Configuration = "Release",
            SelfContained = true,
            NativeArtifactsFallbackPath = Environment.GetEnvironmentVariable(NativeArtifactsFallbackPathEnvVar),
            AllowStaleNativeArtifactsFallback = ParseBooleanEnvironmentVariable(AllowStaleNativeFallbackEnvVar)
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
                case "--force-clean-output":
                    options.ForceCleanOutput = true;
                    index++;
                    break;
                case "--native-artifacts-fallback":
                    options.NativeArtifactsFallbackPath = ReadValue(args, ref index, token);
                    break;
                case "--allow-stale-native-fallback":
                    options.AllowStaleNativeArtifactsFallback = true;
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

    private static bool ParseBooleanEnvironmentVariable(string variableName) {
        string? value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value)) return false;

        return bool.TryParse(value, out bool parsedValue)
            ? parsedValue
            : throw new FormatException($"Environment variable '{variableName}' must be 'true' or 'false'.");
    }
}
