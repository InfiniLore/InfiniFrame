// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.NativeBridge.Parameters.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
/// Compares native application parameters by their configuration values and ABI size.
/// </summary>
internal sealed class InfiniFrameNativeApplicationParametersEqualityComparer
    : IEqualityComparer<InfiniFrameNativeApplicationParameters> {
    internal static readonly InfiniFrameNativeApplicationParametersEqualityComparer Instance = new();

    private InfiniFrameNativeApplicationParametersEqualityComparer() {}

    public bool Equals(
        InfiniFrameNativeApplicationParameters x,
        InfiniFrameNativeApplicationParameters y
    ) => x.WebView2RuntimePath == y.WebView2RuntimePath
        && x.NotificationRegistrationId == y.NotificationRegistrationId
        && x.AppUserModelId == y.AppUserModelId
        && x.DefaultNotificationIcon == y.DefaultNotificationIcon
        && x.Size == y.Size;

    public int GetHashCode(InfiniFrameNativeApplicationParameters obj)
        => HashCode.Combine(
            obj.WebView2RuntimePath,
            obj.NotificationRegistrationId,
            obj.AppUserModelId,
            obj.DefaultNotificationIcon,
            obj.Size
        );
}
