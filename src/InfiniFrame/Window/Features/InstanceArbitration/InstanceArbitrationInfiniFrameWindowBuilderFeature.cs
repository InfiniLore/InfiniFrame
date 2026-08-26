// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder feature implementation for instance arbitration (single-instance enforcement).
/// </summary>
public class InstanceArbitrationInfiniFrameWindowBuilderFeature : IInstanceArbitrationInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IInstanceArbitrationInfiniFrameWindowBuilderFeature.Mode" />
    public InstanceArbitrationMode Mode { get; private set; }

    /// <inheritdoc cref="IInstanceArbitrationInfiniFrameWindowBuilderFeature.MutexName" />
    public string? MutexName { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInstanceArbitrationInfiniFrameWindowBuilderFeature.SetMode" />
    public void SetMode(InstanceArbitrationMode mode) {
        Mode = mode;
    }

    /// <inheritdoc cref="IInstanceArbitrationInfiniFrameWindowBuilderFeature.SetMutexName" />
    public void SetMutexName(string mutexName) {
        MutexName = mutexName;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeature.ApplyToNativeParameters" />
    /// <remarks>
    ///     Instance arbitration is a process-level concern (mutex + elevation detection) and does not
    ///     map to any native window parameters.
    /// </remarks>
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        // No-op: instance arbitration is process-level, not window-level.
    }
}
