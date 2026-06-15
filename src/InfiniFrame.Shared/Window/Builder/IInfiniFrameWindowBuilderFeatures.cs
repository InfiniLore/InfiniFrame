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
    IInfiniFrameWindowBuilderFeatureDebugging Debugging { get; }

    /// <summary>
    ///     Gets the browser feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeatureBrowser Browser { get; }

    /// <summary>
    ///     Gets the decorations feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeatureDecorations Decorations { get; }

    /// <summary>
    ///     Gets the notifications feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeatureNotifications Notifications { get; }

    /// <summary>
    ///     Gets the page navigation feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeaturePageNavigation PageNavigation { get; }

    /// <summary>
    ///     Gets the position feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeaturePosition Position { get; }

    /// <summary>
    ///     Gets the size feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeatureSize Size { get; }

    /// <summary>
    ///     Gets the state feature builder.
    /// </summary>
    IInfiniFrameWindowBuilderFeatureState State { get; }
    
    /// <summary>
    ///     Applies all feature builder configurations to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to update.</param>
    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}
