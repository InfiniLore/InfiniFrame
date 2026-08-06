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

    /// <inheritdoc cref="INotificationsInfiniFrameWindowBuilderFeature.DefaultNotificationIcon"/>
    public string? DefaultNotificationIcon { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="INotificationsInfiniFrameWindowBuilderFeature.EnableNotifications"/>
    public void EnableNotifications(bool enable) {
        IsNotificationsEnabled = enable;
    }

    /// <inheritdoc cref="INotificationsInfiniFrameWindowBuilderFeature.SetDefaultNotificationIcon"/>
    public void SetDefaultNotificationIcon(string? iconPath) {
        DefaultNotificationIcon = iconPath;
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.NotificationsEnabled = IsNotificationsEnabled;
        parameters.DefaultNotificationIcon = DefaultNotificationIcon;
    }
}