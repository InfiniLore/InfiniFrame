// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameNotificationOptionsTests {

    [Test]
    public async Task Constructor_SetsRequiredProperties(CancellationToken ct = default) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Test Title",
            Body = "Test Body"
        };

        // Assert
        await Assert.That(options.Title).IsEqualTo("Test Title");
        await Assert.That(options.Body).IsEqualTo("Test Body");
    }

    [Test]
    public async Task IconPath_DefaultIsNull(CancellationToken ct = default) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body"
        };

        // Assert
        await Assert.That(options.IconPath).IsNull();
    }

    [Test]
    public async Task Urgency_DefaultIsNormal(CancellationToken ct = default) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body"
        };

        // Assert
        await Assert.That(options.Urgency).IsEqualTo(InfiniFrameNotificationUrgency.Normal);
    }

    [Test]
    public async Task Actions_DefaultIsEmptyList(CancellationToken ct = default) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body"
        };

        // Assert
        await Assert.That(options.Actions).IsEmpty();
    }

    [Test]
    public async Task Tag_DefaultIsNull(CancellationToken ct = default) {
        // Arrange & Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body"
        };

        // Assert
        await Assert.That(options.Tag).IsNull();
    }

    [Test]
    public async Task AllProperties_CanBeSet(CancellationToken ct = default) {
        // Arrange
        var actions = new List<InfiniFrameNotificationAction> {
            new("OK", "ok_id"),
            new("Cancel", "cancel_id")
        };

        // Act
        var options = new InfiniFrameNotificationOptions {
            Title = "Title",
            Body = "Body",
            IconPath = "/path/to/icon.png",
            Urgency = InfiniFrameNotificationUrgency.High,
            Actions = actions,
            Tag = "my_tag"
        };

        // Assert
        await Assert.That(options.IconPath).IsEqualTo("/path/to/icon.png");
        await Assert.That(options.Urgency).IsEqualTo(InfiniFrameNotificationUrgency.High);
        await Assert.That(options.Actions.Count).IsEqualTo(2);
        await Assert.That(options.Tag).IsEqualTo("my_tag");
    }
}
