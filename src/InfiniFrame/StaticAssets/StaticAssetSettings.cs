// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class StaticAssetSettings {
    public required IFileProvider FileProvider { get; init; }
    public required string BaseUri { get; init; }
    public required string DefaultDocument { get; init; }
}
