// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface INotificationsInfiniFrameWindowFeature {
    /// <summary>
    ///     Displays a notification with the specified title and body text.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="body">The body text of the notification.</param>
    void ShowNotification(string title, string body);

    /// <summary>
    ///     Displays a rich notification configured through <paramref name="options" />.
    ///     Supports action buttons, custom icons, urgency levels, and notification tagging.
    /// </summary>
    /// <param name="options">The notification configuration.</param>
    void ShowNotification(InfiniFrameNotificationOptions options);

    /// <summary>
    ///     Displays a rich notification and returns a <see cref="Task" /> that completes with the
    ///     user's interaction result. The task resolves when the notification is activated,
    ///     dismissed, timed out, or fails to display.
    /// </summary>
    /// <param name="options">The notification configuration.</param>
    /// <param name="ct">Optional cancellation token to cancel the wait for a response.</param>
    /// <returns>The activation result including which action (if any) was clicked.</returns>
    Task<InfiniFrameNotificationActivation> ShowNotificationAsync(
        InfiniFrameNotificationOptions options,
        CancellationToken ct = default
    );

    /// <summary>
    ///     Displays a message dialog with the specified parameters and returns the user's response.
    /// </summary>
    /// <param name="title">The title of the message dialog.</param>
    /// <param name="text">The optional text content of the message dialog.</param>
    /// <param name="buttons">The button options to display on the dialog.</param>
    /// <param name="icon">The icon to display on the dialog.</param>
    /// <returns>The user's response as an <see cref="InfiniFrameDialogResult" />.</returns>
    InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info);

    /// <summary>Displays a native message dialog and completes when it is answered, canceled, or its owner closes.</summary>
    Task<InfiniFrameDialogResult> ShowMessageAsync(
        string title,
        string? text,
        InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok,
        InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info,
        CancellationToken ct = default
    );
}
