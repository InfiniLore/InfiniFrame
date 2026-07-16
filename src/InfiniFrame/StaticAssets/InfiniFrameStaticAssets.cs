// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameStaticAssets : IInfiniFrameStaticAssets {
    public required IFileProvider FileProvider { get; init; }
    public required string BaseUri { get; init; }
    public required string DefaultDocument { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameStaticAssets DeepCopy() {
        return new InfiniFrameStaticAssets {
            FileProvider = FileProvider,
            BaseUri = BaseUri,
            DefaultDocument = DefaultDocument
        };
    }
}
