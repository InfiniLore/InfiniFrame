// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the result of CLI argument parsing.
/// </summary>
internal sealed class ParseResult {
    /// <summary>
    ///     Gets a value indicating whether usage text should be printed instead of running publish.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Options))]
    public bool ShowUsage { get; private init; }

    /// <summary>
    ///     Gets the process exit code that should be returned by the entrypoint.
    /// </summary>
    public int ExitCode { get; private init; }

    /// <summary>
    ///     Gets parsed publish options when <see cref="ShowUsage" /> is <see langword="false" />.
    /// </summary>
    public PublishOptions? Options { get; private init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Creates a successful parse result with publish options.
    /// </summary>
    /// <param name="options">Resolved options for publish execution.</param>
    /// <returns>A parse result that can be passed to publish.</returns>
    public static ParseResult Success(PublishOptions options) => new() {
        ShowUsage = false,
        ExitCode = ExitCodes.Success,
        Options = options
    };

    /// <summary>
    ///     Creates a parse result that indicates usage should be shown.
    /// </summary>
    /// <param name="exitCode">Exit code returned after printing usage.</param>
    /// <returns>A usage parse result with no publish options.</returns>
    public static ParseResult Usage(int exitCode) => new() {
        ShowUsage = true,
        ExitCode = exitCode
    };
}
