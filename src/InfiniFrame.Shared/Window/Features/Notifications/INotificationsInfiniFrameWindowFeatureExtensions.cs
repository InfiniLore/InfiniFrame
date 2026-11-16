// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="INotificationsInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class INotificationsInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Displays a notification with the specified title and body text and returns the window for chaining.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="body">The body text of the notification.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow ShowNotification(string title, string body) {
            window.Features.Notifications.ShowNotification(title, body);
            return window;
        }
        /// <summary>
        ///     Displays a rich notification configured through <paramref name="options" />
        ///     and returns the window for chaining.
        /// </summary>
        /// <param name="options">The notification configuration.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow ShowNotification(InfiniFrameNotificationOptions options) {
            window.Features.Notifications.ShowNotification(options);
            return window;
        }
        /// <summary>
        ///     Displays a rich notification and returns a <see cref="Task" /> that completes with the
        ///     user's interaction result.
        /// </summary>
        /// <param name="options">The notification configuration.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The activation result.</returns>
        public Task<InfiniFrameNotificationActivation> ShowNotificationAsync(
            InfiniFrameNotificationOptions options,
            CancellationToken ct = default
        ) => window.Features.Notifications.ShowNotificationAsync(options, ct);
        /// <summary>
        ///     Displays a message dialog with the specified parameters and returns the user's response.
        /// </summary>
        /// <param name="title">The title of the message dialog.</param>
        /// <param name="text">The optional text content of the message dialog.</param>
        /// <param name="buttons">The button options to display on the dialog.</param>
        /// <param name="icon">The icon to display on the dialog.</param>
        /// <returns>The user's response as an <see cref="InfiniFrameDialogResult" />.</returns>
        public InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info)
            => window.Features.Notifications.ShowMessage(title, text, buttons, icon);
        /// <summary>Displays a native message dialog and completes when it is answered, canceled, or its owner closes.</summary>
        public Task<InfiniFrameDialogResult> ShowMessageAsync(
            string title,
            string? text,
            InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok,
            InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info,
            CancellationToken ct = default
        ) => window.Features.Notifications.ShowMessageAsync(title, text, buttons, icon, ct);
    }

}
