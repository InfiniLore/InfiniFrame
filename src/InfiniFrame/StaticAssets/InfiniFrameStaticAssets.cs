// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.StaticAssets;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameStaticAssets : IInfiniFrameStaticAssets {
    /// <inheritdoc cref="IInfiniFrameStaticAssets.FileProvider" />
    public required IFileProvider FileProvider { get; init; }
    /// <inheritdoc cref="IInfiniFrameStaticAssets.BaseUri" />
    public required string BaseUri { get; init; }
    /// <inheritdoc cref="IInfiniFrameStaticAssets.DefaultDocument" />
    public required string DefaultDocument { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameStaticAssets.DeepCopy" />
    public IInfiniFrameStaticAssets DeepCopy() =>
        new InfiniFrameStaticAssets {
            FileProvider = FileProvider,
            BaseUri = BaseUri,
            DefaultDocument = DefaultDocument
        };
}
