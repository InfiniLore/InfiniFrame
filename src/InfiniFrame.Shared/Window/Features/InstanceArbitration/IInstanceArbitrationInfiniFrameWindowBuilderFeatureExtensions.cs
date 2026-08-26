// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Extension methods for configuring instance arbitration on the window builder.
/// </summary>
public static class IInstanceArbitrationInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Sets the instance arbitration mode and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="mode">The arbitration mode to apply.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetInstanceArbitrationMode(
        this IInfiniFrameWindowBuilder builder,
        InstanceArbitrationMode mode
    ) {
        builder.Features.InstanceArbitration.SetMode(mode);
        return builder;
    }

    /// <summary>
    ///     Sets the mutex name used for instance arbitration and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="mutexName">The mutex name. Must be unique across applications on the system.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetInstanceArbitrationMutexName(
        this IInfiniFrameWindowBuilder builder,
        string mutexName
    ) {
        builder.Features.InstanceArbitration.SetMutexName(mutexName);
        return builder;
    }
}
