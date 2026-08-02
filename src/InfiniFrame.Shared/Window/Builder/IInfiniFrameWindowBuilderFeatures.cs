// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides access to all feature builders for configuring window features before window creation.
/// </summary>
public interface IInfiniFrameWindowBuilderFeatures {
    /// <summary>
    ///     Gets the debugging feature builder.
    /// </summary>
    IDebuggingInfiniFrameWindowBuilderFeature Debugging { get; }

    /// <summary>
    ///     Gets the browser feature builder.
    /// </summary>
    IBrowserInfiniFrameWindowBuilderFeature Browser { get; }

    /// <summary>
    ///     Gets the decorations feature builder.
    /// </summary>
    IDecorationsInfiniFrameWindowBuilderFeature Decorations { get; }

    /// <summary>
    ///     Gets the notifications feature builder.
    /// </summary>
    INotificationsInfiniFrameWindowBuilderFeature Notifications { get; }

    /// <summary>
    ///     Gets the page navigation feature builder.
    /// </summary>
    IPageNavigationInfiniFrameWindowBuilderFeature PageNavigation { get; }

    /// <summary>
    ///     Gets the position feature builder.
    /// </summary>
    IPositionInfiniFrameWindowBuilderFeature Position { get; }

    /// <summary>
    ///     Gets the size feature builder.
    /// </summary>
    ISizeInfiniFrameWindowBuilderFeature Size { get; }

    /// <summary>
    ///     Gets the state feature builder.
    /// </summary>
    IStateInfiniFrameWindowBuilderFeature State { get; }

    /// <summary>
    ///     Applies all feature builder configurations to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to update.</param>
    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}