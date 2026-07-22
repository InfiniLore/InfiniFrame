// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builds an <see cref="IInfiniFrameWindow"/> by collecting configuration, features, and event handlers.
/// </summary>
public interface IInfiniFrameWindowBuilder : IHasInfiniFrameEventsStore {
    /// <summary>
    ///     Gets or sets the static assets provider for the window.
    /// </summary>
    IInfiniFrameStaticAssets? StaticAssets { get; set; }

    /// <summary>
    ///     Gets the configuration builder for setting window parameters.
    /// </summary>
    IInfiniFrameWindowBuilderConfiguration Configuration { get; }

    /// <summary>
    ///     Gets the debugging feature builder.
    /// </summary>
    IDebuggingInfiniFrameWindowBuilderFeature Debugging { get; }

    /// <summary>
    ///     Gets the features builder for configuring individual window features.
    /// </summary>
    IInfiniFrameWindowBuilderFeatures Features { get; }
    
    /// <summary>
    ///     Builds and returns the <see cref="IInfiniFrameWindow"/> instance.
    /// </summary>
    /// <param name="provider">Optional service provider. If null, a default one is created.</param>
    /// <returns>The constructed <see cref="IInfiniFrameWindow"/>.</returns>
    IInfiniFrameWindow Build(IServiceProvider? provider = null);
}
