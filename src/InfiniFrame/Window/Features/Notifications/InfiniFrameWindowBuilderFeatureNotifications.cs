// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureNotifications : IInfiniFrameWindowBuilderFeatureNotifications {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureNotifications.IsNotificationsEnabled"/>
    public bool IsNotificationsEnabled { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureNotifications.EnableNotifications"/>
    public void EnableNotifications(bool enable) {
        IsNotificationsEnabled = enable;
    }
    
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.NotificationsEnabled = IsNotificationsEnabled;
    }
}