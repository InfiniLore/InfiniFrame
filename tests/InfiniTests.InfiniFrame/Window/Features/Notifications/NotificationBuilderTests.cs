// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NotificationBuilderTests {
    [Test]
    public async Task AtBuilderStage_DefaultNotificationIcon(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Notifications.SetDefaultNotificationIcon("/path/to/icon.png");
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Notifications.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
        await Assert.That(initParameters.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
    }

    [Test]
    public async Task AtBuilderStage_ClearDefaultNotificationIcon(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Notifications.SetDefaultNotificationIcon("/path/to/icon.png");
        builder.Features.Notifications.SetDefaultNotificationIcon(null);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Notifications.DefaultNotificationIcon).IsNull();
        await Assert.That(initParameters.DefaultNotificationIcon).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment_DefaultNotificationIcon(CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetDefaultNotificationIcon("/path/to/icon.png");
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Notifications.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_EnableNotifications_WithDefaultIcon(bool enable, CancellationToken ct) {
        // Arrange
        var builder = new InfiniFrameWindowBuilder();

        // Act
        builder.Features.Notifications.EnableNotifications(enable);
        builder.Features.Notifications.SetDefaultNotificationIcon("/path/to/icon.png");
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Notifications.IsNotificationsEnabled).IsEqualTo(enable);
        await Assert.That(builder.Features.Notifications.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
        await Assert.That(initParameters.NotificationsEnabled).IsEqualTo(enable);
        await Assert.That(initParameters.DefaultNotificationIcon).IsEqualTo("/path/to/icon.png");
    }
}
