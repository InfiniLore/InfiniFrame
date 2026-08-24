// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configuration for a rich notification. Pass an instance to
///     <see cref="INotificationsInfiniFrameWindowFeature.ShowNotification(InfiniFrameNotificationOptions)" />
///     or <see cref="INotificationsInfiniFrameWindowFeature.ShowNotificationAsync" />.
/// </summary>
public sealed class InfiniFrameNotificationOptions {
    /// <summary>
    ///     The title of the notification.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///     The body text of the notification.
    /// </summary>
    public required string Body { get; init; }

    /// <summary>
    ///     Optional path to an image file to display in the notification.
    ///     Supported on Windows (toast image) and macOS (notification attachment).
    ///     Ignored on Linux where libnotify does not support inline images beyond the app icon.
    /// </summary>
    public string? IconPath { get; init; }

    /// <summary>
    ///     Optional urgency level for the notification.
    ///     Default is <see cref="InfiniFrameNotificationUrgency.Normal" />.
    /// </summary>
    public InfiniFrameNotificationUrgency Urgency { get; init; } = InfiniFrameNotificationUrgency.Normal;

    /// <summary>
    ///     Optional action buttons to display on the notification.
    ///     Maximum supported actions varies by platform:
    ///     Windows supports up to 5 actions on toast notifications.
    ///     Linux and macOS have limited or no action button support.
    /// </summary>
    public IReadOnlyList<InfiniFrameNotificationAction> Actions { get; init; } = [];

    /// <summary>
    ///     Optional tag to group or replace previous notifications with the same tag.
    ///     Supported on macOS (notification identifier) and Windows (toast tag).
    ///     Ignored on Linux.
    /// </summary>
    public string? Tag { get; init; }
}
