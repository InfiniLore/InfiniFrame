// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebMessageReceivedEventTests {

    [Test]
    public async Task Record_CanBeConstructed(CancellationToken ct = default) {
        // Arrange & Act
        var evt = new InfiniFrameWebMessageReceivedEvent(
            Message: "hello",
            Origin: "https://example.com"
        );

        // Assert
        await Assert.That(evt.Message).IsEqualTo("hello");
        await Assert.That(evt.Origin).IsEqualTo("https://example.com");
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var evt1 = new InfiniFrameWebMessageReceivedEvent("msg", "https://example.com");
        var evt2 = new InfiniFrameWebMessageReceivedEvent("msg", "https://example.com");

        // Act & Assert
        await Assert.That(evt1).IsEqualTo(evt2);
    }
}
