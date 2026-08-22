// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureWebMessageHandlerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // TryParseRequest
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task TryParseRequest_NullPayload_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(null, out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_EmptyString_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest("", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_WhitespaceOnly_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest("   ", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_InvalidJson_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest("not json", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_EmptyObject_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest("{}", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_MissingCommandProperty_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"args": {}}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_CommandNotString_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": 123}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_InvalidCommandFormat_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "invalid-format"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_WrongPrefix_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__wrong:window:features:size:get"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_TooFewSegments_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_EmptyFeatureName_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features::get"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_EmptyCommandName_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features:size:"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_ValidGetCommand_ParsesCorrectly(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features:size:get"}""", out var request);

        // Assert
        await Assert.That(parsed).IsTrue();
        await Assert.That(request.FeatureName).IsEqualTo("size");
        await Assert.That(request.Command).IsEqualTo("get");
        await Assert.That(request.Args).IsNull();
    }

    [Test]
    public async Task TryParseRequest_ValidPostCommand_ParsesCorrectly(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features:lifecycle:close"}""", out var request);

        // Assert
        await Assert.That(parsed).IsTrue();
        await Assert.That(request.FeatureName).IsEqualTo("lifecycle");
        await Assert.That(request.Command).IsEqualTo("close");
    }

    [Test]
    public async Task TryParseRequest_WithArgs_ParsesArgsElement(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features:size:set", "args": {"width": 800}}""", out var request);

        // Assert
        await Assert.That(parsed).IsTrue();
        await Assert.That(request.Args).IsNotNull();
        await Assert.That(request.Args!.Value.TryGetProperty("width", out JsonElement width)).IsTrue();
        await Assert.That(width.GetInt32()).IsEqualTo(800);
    }

    [Test]
    public async Task TryParseRequest_WithoutArgs_ArgsIsNull(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__infiniframe:window:features:browser:get"}""", out var request);

        // Assert
        await Assert.That(parsed).IsTrue();
        await Assert.That(request.Args).IsNull();
    }

    [Test]
    public async Task TryParseRequest_WrongPrefixSegments_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        // The prefix "__infiniframe:window:features" is matched exactly as string literals
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": "__INFINIFRAME:WINDOW:FEATURES:size:get"}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }

    [Test]
    public async Task TryParseRequest_ArrayValueForCommand_ReturnsFalse(CancellationToken ct = default) {
        // Arrange

        // Act
        bool parsed = WindowFeatureWebMessageHandler.TryParseRequest(
            """{"command": []}""", out var request);

        // Assert
        await Assert.That(parsed).IsFalse();
    }
}
