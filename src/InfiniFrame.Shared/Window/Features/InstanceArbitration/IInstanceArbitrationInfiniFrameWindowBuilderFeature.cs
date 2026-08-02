// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder feature for configuring instance arbitration (single-instance enforcement) before window creation.
/// </summary>
public interface IInstanceArbitrationInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets the instance arbitration mode.
    /// </summary>
    InstanceArbitrationMode Mode { get; }

    /// <summary>
    ///     Gets the mutex name used, for instance, arbitration.
    /// </summary>
    string? MutexName { get; }

    /// <summary>
    ///     Sets the instance arbitration mode.
    /// </summary>
    /// <param name="mode">The arbitration mode to apply.</param>
    void SetMode(InstanceArbitrationMode mode);

    /// <summary>
    ///     Sets the mutex name used for instance arbitration.
    /// </summary>
    /// <param name="mutexName">The mutex name. Must be unique across applications on the system.</param>
    void SetMutexName(string mutexName);
}
