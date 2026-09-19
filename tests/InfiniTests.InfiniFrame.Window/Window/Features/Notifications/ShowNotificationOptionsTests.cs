// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ShowNotificationOptionsTests {
    [Test]
    public async Task NotificationOptions_DefaultValues(CancellationToken ct) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Test",
            Body = "Body"
        };

        // Assert
        await Assert.That(options.Title).IsEqualTo("Test");
        await Assert.That(options.Body).IsEqualTo("Body");
        await Assert.That(options.IconPath).IsNull();
        await Assert.That(options.Urgency).IsEqualTo(InfiniFrameNotificationUrgency.Normal);
        await Assert.That(options.Actions).IsEmpty();
        await Assert.That(options.Tag).IsNull();
    }

    [Test]
    public async Task NotificationOptions_AllProperties(CancellationToken ct) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body",
            IconPath = "/path/to/icon.png",
            Urgency = InfiniFrameNotificationUrgency.High,
            Actions = [
                new InfiniFrameNotificationAction("Show", "show"),
                new InfiniFrameNotificationAction("Hide", "hide")
            ],
            Tag = "my-tag"
        };

        // Assert
        await Assert.That(options.Title).IsEqualTo("Title");
        await Assert.That(options.Body).IsEqualTo("Body");
        await Assert.That(options.IconPath).IsEqualTo("/path/to/icon.png");
        await Assert.That(options.Urgency).IsEqualTo(InfiniFrameNotificationUrgency.High);
        await Assert.That(options.Actions).Count().IsEqualTo(2);
        await Assert.That(options.Tag).IsEqualTo("my-tag");
    }

    [Test]
    public async Task NotificationAction_RecordEquality(CancellationToken ct) {
        // Arrange & Act
        var action1 = new InfiniFrameNotificationAction("Label", "id");
        var action2 = new InfiniFrameNotificationAction("Label", "id");
        var action3 = new InfiniFrameNotificationAction("Other", "id");

        // Assert
        await Assert.That(action1).IsEqualTo(action2);
        await Assert.That(action1).IsNotEqualTo(action3);
    }

    [Test]
    public async Task NotificationActivation_RecordEquality(CancellationToken ct) {
        // Arrange & Act
        var activation1 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.ActionClicked, "show");
        var activation2 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.ActionClicked, "show");
        var activation3 = new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed);

        // Assert
        await Assert.That(activation1).IsEqualTo(activation2);
        await Assert.That(activation1).IsNotEqualTo(activation3);
    }

    [Test]
    [Arguments(InfiniFrameNotificationUrgency.Normal)]
    [Arguments(InfiniFrameNotificationUrgency.Low)]
    [Arguments(InfiniFrameNotificationUrgency.High)]
    [Arguments(InfiniFrameNotificationUrgency.Critical)]
    public async Task NotificationUrgency_AllValuesDefined(InfiniFrameNotificationUrgency urgency, CancellationToken ct) {
        // Assert
        await Assert.That(Enum.IsDefined(urgency)).IsTrue();
    }

    [Test]
    [Arguments(InfiniFrameNotificationResult.Dismissed)]
    [Arguments(InfiniFrameNotificationResult.BodyClicked)]
    [Arguments(InfiniFrameNotificationResult.ActionClicked)]
    [Arguments(InfiniFrameNotificationResult.TimedOut)]
    [Arguments(InfiniFrameNotificationResult.Failed)]
    public async Task NotificationResult_AllValuesDefined(InfiniFrameNotificationResult result, CancellationToken ct) {
        // Assert
        await Assert.That(Enum.IsDefined(result)).IsTrue();
    }
}
