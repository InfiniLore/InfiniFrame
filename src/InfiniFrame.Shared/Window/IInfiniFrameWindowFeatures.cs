// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatures {
    IInfiniFrameWindowFeatureLifecycle Lifecycle { get; }
    IInfiniFrameWindowFeatureInvoke Invoke { get; }
    IInfiniFrameWindowFeatureWebMessaging WebMessaging { get; }
    IInfiniFrameWindowFeatureNotifications Notifications { get; }
    IInfiniFrameWindowFeatureFilePickerDialogs FilePickerDialogs { get; }
    IInfiniFrameWindowFeatureMonitors Monitors { get; }
    IInfiniFrameWindowFeaturePageNavigation PageNavigation { get; }
    IInfiniFrameWindowFeaturePosition Position { get; }
    IInfiniFrameWindowFeatureSize Size { get; }
    IInfiniFrameWindowFeatureDecorations Decorations { get; }
    IInfiniFrameWindowFeatureState State { get; }
    IInfiniFrameWindowFeatureBrowser Browser { get; }
}