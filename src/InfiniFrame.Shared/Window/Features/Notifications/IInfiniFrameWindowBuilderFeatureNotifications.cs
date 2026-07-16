// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureNotifications : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets whether notifications are enabled for the window.
    /// </summary>
    bool IsNotificationsEnabled { get; }

    /// <summary>
    ///     Enables or disables notifications for the window.
    /// </summary>
    /// <param name="enable">Whether to enable notifications.</param>
    void EnableNotifications(bool enable);
}
