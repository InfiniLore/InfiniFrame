// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the outcome of a notification interaction.
/// </summary>
public enum InfiniFrameNotificationResult {
    /// <summary>
    ///     The notification was dismissed without activation (timeout, swipe, or programmatic dismiss).
    /// </summary>
    Dismissed,

    /// <summary>
    ///     The user clicked the notification body or the window was brought to the foreground.
    /// </summary>
    BodyClicked,

    /// <summary>
    ///     The user clicked an action button. The <see cref="InfiniFrameNotificationActivation.ActionIdentifier"/>
    ///     field identifies which action was activated.
    /// </summary>
    ActionClicked,

    /// <summary>
    ///     The notification timed out before the user interacted with it.
    /// </summary>
    TimedOut,

    /// <summary>
    ///     The notification failed to display due to a platform error.
    /// </summary>
    Failed
}
