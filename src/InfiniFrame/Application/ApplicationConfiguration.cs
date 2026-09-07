namespace InfiniFrame;

internal sealed record ApplicationConfiguration(
    string? WebView2RuntimePath = null,
    string? NotificationRegistrationId = null,
    string? AppUserModelId = null,
    string? DefaultNotificationIcon = null
);
