// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class NotificationsExtensionMethodTests {

    [Test]
    public async Task ShowNotificationWithTitleBody_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<INotificationsInfiniFrameWindowFeature> notifications = MockFactory.CreateNotificationsMock();
        window.Features.Returns(features.Object);
        features.Notifications.Returns(notifications.Object);

        // Act
        IInfiniFrameWindow result = window.Object.ShowNotification("Title", "Body");

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task ShowNotificationWithOptions_ReturnsWindowForChaining(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<INotificationsInfiniFrameWindowFeature> notifications = MockFactory.CreateNotificationsMock();
        window.Features.Returns(features.Object);
        features.Notifications.Returns(notifications.Object);
        var options = new InfiniFrameNotificationOptions { Title = "T", Body = "B" };

        // Act
        IInfiniFrameWindow result = window.Object.ShowNotification(options);

        // Assert
        await Assert.That(result).IsSameReferenceAs(window.Object);
    }

    [Test]
    public async Task ShowMessage_DelegatesToFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<INotificationsInfiniFrameWindowFeature> notifications = MockFactory.CreateNotificationsMock();
        window.Features.Returns(features.Object);
        features.Notifications.Returns(notifications.Object);

        // Act
        InfiniFrameDialogResult result = window.Object.ShowMessage("Title", "Text");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Ok);
    }
}
