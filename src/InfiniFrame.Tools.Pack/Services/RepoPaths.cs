// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class RepoPaths {
    public required string RepoRoot { get; init; }
    public required string NativeProjectPath { get; init; }
    public required string NativeArtifactsDir { get; init; }
    public required string NativePlatform { get; init; }
}
