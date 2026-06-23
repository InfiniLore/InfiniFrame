// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Aggregates all builder-level feature configurations that are applied to native parameters before window creation.
/// </summary>
public sealed class InfiniFrameWindowBuilderFeatures : IInfiniFrameWindowBuilderFeatures {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Debugging"/>
    public IInfiniFrameWindowBuilderFeatureDebugging Debugging { get; } = new InfiniFrameWindowBuilderFeatureDebugging();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Browser"/>
    public IInfiniFrameWindowBuilderFeatureBrowser Browser { get; } = new InfiniFrameWindowBuilderFeatureBrowser();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Decorations"/>
    public IInfiniFrameWindowBuilderFeatureDecorations Decorations { get; } = new InfiniFrameWindowBuilderFeatureDecorations();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Notifications"/>
    public IInfiniFrameWindowBuilderFeatureNotifications Notifications { get; } = new InfiniFrameWindowBuilderFeatureNotifications();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.PageNavigation"/>
    public IInfiniFrameWindowBuilderFeaturePageNavigation PageNavigation { get; } = new InfiniFrameWindowBuilderFeaturePageNavigation();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Position"/>
    public IInfiniFrameWindowBuilderFeaturePosition Position { get; } = new InfiniFrameWindowBuilderFeaturePosition();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Size"/>
    public IInfiniFrameWindowBuilderFeatureSize Size { get; } = new InfiniFrameWindowBuilderFeatureSize();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.State"/>
    public IInfiniFrameWindowBuilderFeatureState State { get; } = new InfiniFrameWindowBuilderFeatureState();
    

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.ApplyToNativeParameters"/>
    /// <summary>
    ///     Applies all configured feature settings to the native parameters.
    /// </summary>
    /// <param name="parameters">The native parameters to populate.</param>
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        Debugging.ApplyToNativeParameters(ref parameters);
        Browser.ApplyToNativeParameters(ref parameters);
        Decorations.ApplyToNativeParameters(ref parameters);
        Notifications.ApplyToNativeParameters(ref parameters);
        PageNavigation.ApplyToNativeParameters(ref parameters);
        Position.ApplyToNativeParameters(ref parameters);
        Size.ApplyToNativeParameters(ref parameters);
        State.ApplyToNativeParameters(ref parameters);
    }
}
