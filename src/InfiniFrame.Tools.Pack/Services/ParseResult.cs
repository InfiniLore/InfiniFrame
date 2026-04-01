// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class ParseResult {
    [MemberNotNullWhen(false, nameof(Options))]
    public bool ShowUsage { get; private init; }
    
    public int ExitCode { get; private init; }
    public PublishOptions? Options { get; private init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
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
