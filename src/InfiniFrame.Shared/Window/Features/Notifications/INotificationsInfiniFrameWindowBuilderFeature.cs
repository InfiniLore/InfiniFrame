// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface INotificationsInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets whether notifications are enabled for the window.
    /// </summary>
    bool IsNotificationsEnabled { get; }

    /// <summary>
    ///     Enables or disables notifications for the window.
    /// </summary>
    /// <param name="enable">Whether to enable notifications.</param>
    void EnableNotifications(bool enable);

    /// <summary>
    ///     Gets the default icon path applied to notifications when
    ///     <see cref="InfiniFrameNotificationOptions.IconPath"/> is not set.
    /// </summary>
    string? DefaultNotificationIcon { get; }

    /// <summary>
    ///     Sets the default icon path applied to notifications when
    ///     <see cref="InfiniFrameNotificationOptions.IconPath"/> is not set.
    /// </summary>
    /// <param name="iconPath">Absolute path to an image file, or <c>null</c> to clear.</param>
    void SetDefaultNotificationIcon(string? iconPath);
}