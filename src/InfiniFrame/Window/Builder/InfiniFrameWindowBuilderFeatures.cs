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
    public IDebuggingInfiniFrameWindowBuilderFeature Debugging { get; } = new DebuggingInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Browser"/>
    public IBrowserInfiniFrameWindowBuilderFeature Browser { get; } = new BrowserInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Decorations"/>
    public IDecorationsInfiniFrameWindowBuilderFeature Decorations { get; } = new DecorationsInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Notifications"/>
    public INotificationsInfiniFrameWindowBuilderFeature Notifications { get; } = new NotificationsInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.PageNavigation"/>
    public IPageNavigationInfiniFrameWindowBuilderFeature PageNavigation { get; } = new PageNavigationInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Position"/>
    public IPositionInfiniFrameWindowBuilderFeature Position { get; } = new PositionInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.Size"/>
    public ISizeInfiniFrameWindowBuilderFeature Size { get; } = new SizeInfiniFrameWindowBuilderFeature();
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatures.State"/>
    public IStateInfiniFrameWindowBuilderFeature State { get; } = new StateInfiniFrameWindowBuilderFeature();


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