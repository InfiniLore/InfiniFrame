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
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetNotificationsEnabled
    );


    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureNotifications.ShowNotification" />
    public void ShowNotification(string title, string body) {
        if (window.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.ShowNotification,
            title,
            body
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureNotifications.ShowMessage" />
    public InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info) {
        if (window.IsClosedOrClosing()) return InfiniFrameDialogResult.Cancel;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
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
