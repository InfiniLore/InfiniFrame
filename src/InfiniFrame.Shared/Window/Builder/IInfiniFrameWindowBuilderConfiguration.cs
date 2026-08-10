// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configures parent/child window relationships for the window being built.
/// </summary>
public interface IInfiniFrameWindowBuilderConfiguration {
    /// <summary>
    ///     Gets the parent window to assign to the window being built.
    /// </summary>
    IInfiniFrameWindow? ParentWindow { get; }

    /// <summary>
    ///     Gets the list of child windows to associate with the window being built.
    /// </summary>
    IReadOnlyList<IInfiniFrameWindow> ChildWindows { get; }

    /// <summary>
    ///     Applies the builder configuration values to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to update.</param>
    void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}