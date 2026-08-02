// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides configuration data for an InfiniFrame window, including startup parameters and parent/child window relationships.
/// </summary>
public interface IInfiniFrameWindowConfiguration {
    /// <summary>
    ///     Gets the native parameters used when starting the window.
    /// </summary>
    InfiniFrameNativeParameters StartupParameters { get; }

    /// <summary>
    ///     Gets or sets the parent window of this window.
    /// </summary>
    IInfiniFrameWindow? ParentWindow { get; internal set; }

    /// <summary>
    ///     Gets the list of child windows associated with this window.
    /// </summary>
    List<IInfiniFrameWindow> ChildWindows { get; }

    /// <summary>
    ///     Assigns the native parameters to this configuration.
    /// </summary>
    /// <param name="nativeParameters">The native parameters to assign.</param>
    internal void AssignNativeParameters(InfiniFrameNativeParameters nativeParameters);
}