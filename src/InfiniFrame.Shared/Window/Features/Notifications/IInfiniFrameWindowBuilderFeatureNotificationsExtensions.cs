// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeatureNotificationsExtensions {
    public static IInfiniFrameWindowBuilder EnableNotifications(this IInfiniFrameWindowBuilder builder, bool enable)   {
        builder.Features.Notifications.EnableNotifications(enable);
        return builder;
    }
}