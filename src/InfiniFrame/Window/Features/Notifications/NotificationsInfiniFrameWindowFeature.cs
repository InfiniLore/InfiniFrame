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
public class NotificationsInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<NotificationsInfiniFrameWindowFeature> logger
) : INotificationsInfiniFrameWindowFeature {

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
    /// <inheritdoc cref="INotificationsInfiniFrameWindowFeature.ShowNotification" />
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

    /// <inheritdoc cref="INotificationsInfiniFrameWindowFeature.ShowMessage" />
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

    /// <inheritdoc cref="INotificationsInfiniFrameWindowFeature.ShowMessageAsync" />
    public async Task<InfiniFrameDialogResult> ShowMessageAsync(
        string title, string? text,
        InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok,
        InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info,
        CancellationToken ct = default
    ) {
        ct.ThrowIfCancellationRequested();
        if (window.IsClosedOrClosing()) return InfiniFrameDialogResult.Cancel;
        var operation = new InfiniMessageDialogOperation(
            window, logger, title, text ?? string.Empty, buttons, icon, ct
        );
        _ = operation.StartAsync();
        return await operation.Task.WaitAsync(ct).ConfigureAwait(false);
    }
}
