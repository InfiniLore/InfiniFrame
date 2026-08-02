// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NotificationsInfiniFrameWindowBuilderFeature : INotificationsInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="INotificationsInfiniFrameWindowBuilderFeature.IsNotificationsEnabled"/>
    public bool IsNotificationsEnabled { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="INotificationsInfiniFrameWindowBuilderFeature.EnableNotifications"/>
    public void EnableNotifications(bool enable) {
        IsNotificationsEnabled = enable;
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.NotificationsEnabled = IsNotificationsEnabled;
    }
}