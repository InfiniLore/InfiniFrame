// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Dialogs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureNotifications(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureNotifications> logger
) : IInfiniFrameWindowFeatureNotifications {
    
    public string? NotificationRegistrationId => window.Configuration.StartupParameters.NotificationRegistrationId;

    [SupportedOSPlatform("windows")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool NotificationsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetNotificationsEnabled
    );

    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a notification with the specified title and body text.
    /// </summary>
    /// <param name="title">The title of the notification.</param>
    /// <param name="body">The body text of the notification.</param>
    public void ShowNotification(string title, string body) {
        if (window.IsClosedOrClosing()) return;
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle, 
            window.ManagedThreadId,
            InfiniFrameNative.ShowNotification,
            title,
            body
        );
    }

    /// <summary>
    /// Displays a message dialog with the specified parameters and returns the user's response.
    /// </summary>
    /// <param name="title">The title of the message dialog.</param>
    /// <param name="text">The optional text content of the message dialog.</param>
    /// <param name="buttons">The button options to display on the dialog. Defaults to Ok.</param>
    /// <param name="icon">The icon to display on the dialog. Defaults to Info.</param>
    /// <returns>The user's response as an <see cref="InfiniFrameDialogResult"/>.</returns>
    public InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info) {
        if (window.IsClosedOrClosing()) return InfiniFrameDialogResult.Cancel;
        
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle, 
            window.ManagedThreadId,
            InfiniFrameNative.ShowMessage,
            title,
            text ?? string.Empty,
            buttons,
            icon,
            out InfiniFrameDialogResult result
        );
        
        return result;
    }
}