// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeatureNotificationsExtensions {
    public static IInfiniFrameWindowBuilder SetNotificationsEnabled(this IInfiniFrameWindowBuilder builder, bool enable)   {
        builder.Features.Notifications.SetNotificationsEnabled(enable);
        return builder;
    }
}