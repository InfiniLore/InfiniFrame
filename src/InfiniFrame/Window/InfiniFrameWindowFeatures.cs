// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record InfiniFrameWindowFeatures(
    IInfiniFrameWindowFeatureLifecycle Lifecycle,
    IInfiniFrameWindowFeatureInvoke Invoke,
    IInfiniFrameWindowFeatureWebMessaging WebMessaging,
    IInfiniFrameWindowFeatureNotifications Notifications,
    IInfiniFrameWindowFeatureFilePickerDialogs FilePickerDialogs,
    IInfiniFrameWindowFeatureMonitors Monitors,
    IInfiniFrameWindowFeaturePageNavigation PageNavigation,
    IInfiniFrameWindowFeaturePosition Position,
    IInfiniFrameWindowFeatureSize Size,
    IInfiniFrameWindowFeatureDecorations Decorations,
    IInfiniFrameWindowFeatureState State,
    IInfiniFrameWindowFeatureBrowser Browser
) : IInfiniFrameWindowFeatures;
