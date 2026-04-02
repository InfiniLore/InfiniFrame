// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Contains repository-relative paths and platform metadata used during publish.
/// </summary>
internal sealed class RepoPaths {
    /// <summary>
    /// Gets the repository root that contains <c>src/InfiniFrame.Native/InfiniFrame.Native.proj</c>.
    /// </summary>
    public required string RepoRoot { get; init; }

    /// <summary>
    /// Gets the full path to the native runtime project file.
    /// </summary>
    public required string NativeProjectPath { get; init; }

    /// <summary>
    /// Gets the directory where native build artifacts are expected for the RID and configuration.
    /// </summary>
    public required string NativeArtifactsDir { get; init; }

    /// <summary>
    /// Gets the native platform value used when building the native project.
    /// </summary>
    public required string NativePlatform { get; init; }
}
