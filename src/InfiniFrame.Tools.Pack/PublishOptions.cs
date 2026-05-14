// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents publish options accepted by the <c>publish</c> command.
/// </summary>
internal sealed class PublishOptions {
    /// <summary>
    ///     Gets or sets the path to the project file to publish.
    /// </summary>
    public required string ProjectPath { get; set; }

    /// <summary>
    ///     Gets or sets the target runtime identifier or <c>auto</c>.
    /// </summary>
    public required string Rid { get; set; }

    /// <summary>
    ///     Gets or sets the build configuration.
    /// </summary>
    public required string Configuration { get; set; }

    /// <summary>
    ///     Gets or sets the target framework. When omitted, the framework is resolved from the project file.
    /// </summary>
    public string? Framework { get; set; }

    /// <summary>
    ///     Gets or sets whether publish output is self-contained.
    /// </summary>
    public required bool SelfContained { get; set; }

    /// <summary>
    ///     Gets or sets the output directory. When omitted, a default publish path under <c>bin</c> is used.
    /// </summary>
    public string? Output { get; set; }

    /// <summary>
    ///     Gets or sets whether restore should be skipped for the publish command.
    /// </summary>
    public bool NoRestore { get; set; }

    /// <summary>
    ///     Gets or sets whether verbose process output should be enabled.
    /// </summary>
    public bool Verbose { get; set; }

    /// <summary>
    ///     Gets or sets whether the tool may recursively delete a non-default output directory before publish.
    /// </summary>
    public bool ForceCleanOutput { get; set; }

}
