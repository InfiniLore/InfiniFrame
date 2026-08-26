// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides access to static assets embedded in the application assembly.
/// </summary>
public interface IInfiniFrameStaticAssets {
    /// <summary>
    ///     Gets the file provider used to serve static assets.
    /// </summary>
    IFileProvider FileProvider { get; }

    /// <summary>
    ///     Gets the base URI for static assets.
    /// </summary>
    string BaseUri { get; }

    /// <summary>
    ///     Gets the default document name served when no path is specified.
    /// </summary>
    string DefaultDocument { get; }

    /// <summary>
    ///     Creates a shallow copy of the static assets configuration.
    ///     The returned instance shares the same <see cref="FileProvider" /> reference.
    /// </summary>
    /// <returns>A new <see cref="IInfiniFrameStaticAssets" /> instance with the same property values.</returns>
    IInfiniFrameStaticAssets DeepCopy();
}
