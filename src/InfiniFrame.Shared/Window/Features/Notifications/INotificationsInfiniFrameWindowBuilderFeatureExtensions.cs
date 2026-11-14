// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="INotificationsInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class INotificationsInfiniFrameWindowBuilderFeatureExtensions {
    /// <param name="builder">The window builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Enables or disables notifications for the window and returns the builder for chaining.
        /// </summary>
        /// <param name="enable">Whether to enable notifications.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder EnableNotifications(bool enable) {
            builder.Features.Notifications.EnableNotifications(enable);
            return builder;
        }
        /// <summary>
        ///     Sets the default icon path applied to notifications and returns the builder for chaining.
        /// </summary>
        /// <param name="iconPath">Absolute path to an image file, or <c>null</c> to clear.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetDefaultNotificationIcon(string? iconPath) {
            builder.Features.Notifications.SetDefaultNotificationIcon(iconPath);
            return builder;
        }
    }

}
