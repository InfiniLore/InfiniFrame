// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed record ApplicationConfiguration(
    string? WebView2RuntimePath = null,
    string? NotificationRegistrationId = null,
    string? AppUserModelId = null,
    string? DefaultNotificationIcon = null
) {
    internal InfiniFrameNativeApplicationParameters ToNativeParameters() => new() {
        WebView2RuntimePath = WebView2RuntimePath,
        NotificationRegistrationId = NotificationRegistrationId,
        AppUserModelId = AppUserModelId,
        DefaultNotificationIcon = DefaultNotificationIcon
    };
}
