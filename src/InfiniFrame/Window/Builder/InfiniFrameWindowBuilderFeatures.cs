// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowBuilderFeatures : IInfiniFrameWindowBuilderFeatures {
    public IInfiniFrameWindowBuilderFeatureBrowser Browser { get; } = new InfiniFrameWindowBuilderFeatureBrowser();
    public IInfiniFrameWindowBuilderFeatureDecorations Decorations { get; } = new InfiniFrameWindowBuilderFeatureDecorations();
    public IInfiniFrameWindowBuilderFeatureNotifications Notifications { get; } = new InfiniFrameWindowBuilderFeatureNotifications();
    public IInfiniFrameWindowBuilderFeaturePageNavigation PageNavigation { get; } = new InfiniFrameWindowBuilderFeaturePageNavigation();
    public IInfiniFrameWindowBuilderFeaturePosition Position { get; } = new InfiniFrameWindowBuilderFeaturePosition();
    public IInfiniFrameWindowBuilderFeatureSize Size { get; } = new InfiniFrameWindowBuilderFeatureSize();
    public IInfiniFrameWindowBuilderFeatureState State { get; } = new InfiniFrameWindowBuilderFeatureState();
}
