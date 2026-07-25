// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class INotificationsInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Enables or disables notifications for the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="enable">Whether to enable notifications.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableNotifications(this IInfiniFrameWindowBuilder builder, bool enable)   {
        builder.Features.Notifications.EnableNotifications(enable);
        return builder;
    }
}