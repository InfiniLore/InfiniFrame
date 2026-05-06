// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class StaticAssetSettings {
    public required IFileProvider FileProvider { get; init; }
    public required string BaseUri { get; init; }
    public required string DefaultDocument { get; init; }
    
    public StaticAssetSettings DeepCopy() {
        return new StaticAssetSettings {
            FileProvider = FileProvider,
            BaseUri = BaseUri,
            DefaultDocument = DefaultDocument
        };
    }
}
