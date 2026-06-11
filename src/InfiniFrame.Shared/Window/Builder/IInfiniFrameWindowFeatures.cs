// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatures {
    IInfiniFrameWindowBuilderFeatureBrowser Browser { get; }
    IInfiniFrameWindowBuilderFeatureDecorations Decorations { get; }
    IInfiniFrameWindowBuilderFeatureNotifications Notifications { get; }
    IInfiniFrameWindowBuilderFeaturePageNavigation PageNavigation { get; }
    IInfiniFrameWindowBuilderFeaturePosition Position { get; }
    IInfiniFrameWindowBuilderFeatureSize Size { get; }
    IInfiniFrameWindowBuilderFeatureState State { get; }
}