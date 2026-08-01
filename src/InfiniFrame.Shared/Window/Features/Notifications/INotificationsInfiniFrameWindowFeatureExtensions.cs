// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class INotificationsInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Displays a notification with the specified title and body text and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The title of the notification.</param>
    /// <param name="body">The body text of the notification.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow ShowNotification(this IInfiniFrameWindow window, string title, string body) {
        window.Features.Notifications.ShowNotification(title, body);
        return window;
    }

    /// <summary>
    ///     Displays a message dialog with the specified parameters and returns the user's response.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The title of the message dialog.</param>
    /// <param name="text">The optional text content of the message dialog.</param>
    /// <param name="buttons">The button options to display on the dialog.</param>
    /// <param name="icon">The icon to display on the dialog.</param>
    /// <returns>The user's response as an <see cref="InfiniFrameDialogResult"/>.</returns>
    public static InfiniFrameDialogResult ShowMessage(this IInfiniFrameWindow window, string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info)
        => window.Features.Notifications.ShowMessage(title, text, buttons, icon);

    /// <summary>Displays a native message dialog and completes when it is answered, cancelled, or its owner closes.</summary>
    public static Task<InfiniFrameDialogResult> ShowMessageAsync(
        this IInfiniFrameWindow window, string title, string? text,
        InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok,
        InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info,
        CancellationToken ct = default
    ) => window.Features.Notifications.ShowMessageAsync(title, text, buttons, icon, ct);
}
