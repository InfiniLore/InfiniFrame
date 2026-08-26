// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the urgency level of a notification.
///     Platform support varies: Windows maps this to toast importance, Linux maps to libnotify urgency,
///     and macOS maps to interruption level (iOS 15+ / macOS 12+).
/// </summary>
public enum InfiniFrameNotificationUrgency {
    /// <summary>
    ///     Normal urgency. The notification is shown with default platform behavior.
    /// </summary>
    Normal,

    /// <summary>
    ///     Low urgency. The notification may be silently delivered or placed in a notification center
    ///     without interrupting the user.
    /// </summary>
    Low,

    /// <summary>
    ///     High urgency. The notification may trigger a sound or vibration.
    /// </summary>
    High,

    /// <summary>
    ///     Critical urgency. The notification interrupts the user immediately.
    ///     Not supported on all platforms; falls back to <see cref="High" /> where unavailable.
    /// </summary>
    Critical
}
