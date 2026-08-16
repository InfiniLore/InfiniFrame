// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InteropEnvelopeProtocolCreateTests {

    [Test]
    public async Task CreateEnvelopeMessage_DefaultCommand_IsPost(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id");

        // Assert
        await Assert.That(message).Contains("\"command\":\"Post\"");
    }

    [Test]
    public async Task CreateEnvelopeMessage_WithGetCommand(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id", command: "Get");

        // Assert
        await Assert.That(message).Contains("\"command\":\"Get\"");
    }

    [Test]
    public async Task CreateEnvelopeMessage_IncludesVersion(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id");

        // Assert
        await Assert.That(message).Contains("\"version\":2");
    }

    [Test]
    public async Task CreateEnvelopeMessage_NullData_WritesNull(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id", data: null);

        // Assert
        await Assert.That(message).Contains("\"data\":null");
    }

    [Test]
    public async Task CreateEnvelopeMessage_WithData_WritesString(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id", data: "hello world");

        // Assert
        await Assert.That(message).Contains("\"data\":\"hello world\"");
    }

    [Test]
    public async Task CreateEnvelopeMessage_WithRequestId_IncludesRequestId(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id", requestId: "req-123");

        // Assert
        await Assert.That(message).Contains("\"requestId\":\"req-123\"");
    }

    [Test]
    public async Task CreateEnvelopeMessage_WithoutRequestId_OmitsRequestId(CancellationToken ct = default) {
        // Arrange & Act
        string message = InteropEnvelopeProtocol.CreateEnvelopeMessage("test-id");

        // Assert
        await Assert.That(message).DoesNotContain("requestId");
    }

    [Test]
    public async Task CreateEnvelopeMessage_EmptyId_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => InteropEnvelopeProtocol.CreateEnvelopeMessage(""))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task CreateEnvelopeMessage_NullId_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => InteropEnvelopeProtocol.CreateEnvelopeMessage(null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task CreateEnvelopeMessage_EmptyCommand_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange & Act & Assert
        await Assert.That(() => InteropEnvelopeProtocol.CreateEnvelopeMessage("id", command: ""))
            .Throws<ArgumentException>();
    }
}
