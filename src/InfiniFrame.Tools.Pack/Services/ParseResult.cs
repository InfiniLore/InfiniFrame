// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class ParseResult {
    public bool ShowUsage { get; init; }
    public int ExitCode { get; init; }
    public PublishOptions? Options { get; init; }

    public static ParseResult Success(PublishOptions options) => new() {
        ShowUsage = false,
        ExitCode = 0,
        Options = options
    };

    public static ParseResult Usage(int exitCode) => new() {
        ShowUsage = true,
        ExitCode = exitCode
    };
}
