// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureNotificationsExtensions {
    public static IInfiniFrameWindow ShowNotification(this IInfiniFrameWindow window, string title, string body) {
        window.Features.Notifications.ShowNotification(title, body);
        return window;
    }

    public static InfiniFrameDialogResult ShowMessage(this IInfiniFrameWindow window, string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info)
        => window.Features.Notifications.ShowMessage(title, text, buttons, icon);
}
