// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Base interface for builder features that can apply their configuration to native parameters.
/// </summary>
public interface IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Applies the feature's configuration values to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to update.</param>
    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}