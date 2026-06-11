// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowBuilderFeatures : IInfiniFrameWindowBuilderFeatures {
    public IInfiniFrameWindowBuilderFeatureDebugging Debugging { get; } = new InfiniFrameWindowBuilderFeatureDebugging();
    public IInfiniFrameWindowBuilderFeatureBrowser Browser { get; } = new InfiniFrameWindowBuilderFeatureBrowser();
    public IInfiniFrameWindowBuilderFeatureDecorations Decorations { get; } = new InfiniFrameWindowBuilderFeatureDecorations();
    public IInfiniFrameWindowBuilderFeatureNotifications Notifications { get; } = new InfiniFrameWindowBuilderFeatureNotifications();
    public IInfiniFrameWindowBuilderFeaturePageNavigation PageNavigation { get; } = new InfiniFrameWindowBuilderFeaturePageNavigation();
    public IInfiniFrameWindowBuilderFeaturePosition Position { get; } = new InfiniFrameWindowBuilderFeaturePosition();
    public IInfiniFrameWindowBuilderFeatureSize Size { get; } = new InfiniFrameWindowBuilderFeatureSize();
    public IInfiniFrameWindowBuilderFeatureState State { get; } = new InfiniFrameWindowBuilderFeatureState();
    

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
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
