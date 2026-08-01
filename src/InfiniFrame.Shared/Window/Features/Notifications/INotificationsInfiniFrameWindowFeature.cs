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
    ///     Displays a message dialog with the specified parameters and returns the user's response.
    /// </summary>
    /// <param name="title">The title of the message dialog.</param>
    /// <param name="text">The optional text content of the message dialog.</param>
    /// <param name="buttons">The button options to display on the dialog.</param>
    /// <param name="icon">The icon to display on the dialog.</param>
    /// <returns>The user's response as an <see cref="InfiniFrameDialogResult"/>.</returns>
    InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info);

    /// <summary>Displays a native message dialog and completes when it is answered, cancelled, or its owner closes.</summary>
    Task<InfiniFrameDialogResult> ShowMessageAsync(
        string title, string? text,
        InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok,
        InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info,
        CancellationToken ct = default
    );
}
