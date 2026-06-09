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
}
