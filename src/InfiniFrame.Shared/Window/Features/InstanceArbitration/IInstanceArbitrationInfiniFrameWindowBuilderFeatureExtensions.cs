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
    /// <param name="builder">The window builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Sets the instance arbitration mode and returns the builder for chaining.
        /// </summary>
        /// <param name="mode">The arbitration mode to apply.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetInstanceArbitrationMode(
            InstanceArbitrationMode mode
        ) {
            builder.Features.InstanceArbitration.SetMode(mode);
            return builder;
        }
        /// <summary>
        ///     Sets the mutex name used for instance arbitration and returns the builder for chaining.
        /// </summary>
        /// <param name="mutexName">The mutex name. Must be unique across applications on the system.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetInstanceArbitrationMutexName(
            string mutexName
        ) {
            builder.Features.InstanceArbitration.SetMutexName(mutexName);
            return builder;
        }
    }

}
