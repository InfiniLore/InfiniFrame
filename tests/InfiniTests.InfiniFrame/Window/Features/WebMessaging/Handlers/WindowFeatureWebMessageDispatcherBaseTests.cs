// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureWebMessageDispatcherBaseTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Required Method Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Required_MissingArgument_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"other": "value"}""");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestRequired<string>(args, "name"));

        await Assert.That(exception.Message).IsEqualTo("Argument 'name' is required.");
    }

    [Test]
    public async Task Required_NullArgs_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestRequired<string>(args, "name"));

        await Assert.That(exception.Message).IsEqualTo("Argument 'name' is required.");
    }

    [Test]
    public async Task Required_EmptyObject_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{}""");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestRequired<string>(args, "name"));

        await Assert.That(exception.Message).IsEqualTo("Argument 'name' is required.");
    }

    [Test]
    public async Task Required_NullValue_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"name": null}""");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestRequired<string>(args, "name"));

        await Assert.That(exception.Message).IsEqualTo("Argument 'name' cannot be null.");
    }

    [Test]
    public async Task Required_ValidStringArgument_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"name": "hello"}""");

        // Act
        string result = TestDispatcher.TestRequired<string>(args, "name");

        // Assert
        await Assert.That(result).IsEqualTo("hello");
    }

    [Test]
    public async Task Required_ValidIntArgument_ReturnsValue(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"count": 42}""");

        // Act
        int result = TestDispatcher.TestRequired<int>(args, "count");

        // Assert
        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public async Task Required_WrongType_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"value": "not-a-number"}""");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestRequired<int>(args, "value"));

        await Assert.That(exception.Message).Contains("Argument 'value' is invalid.");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Arg Method Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Arg_MissingArgument_ReturnsFallback(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"other": "value"}""");

        // Act
        string result = TestDispatcher.TestArg(args, "name", "fallback");

        // Assert
        await Assert.That(result).IsEqualTo("fallback");
    }

    [Test]
    public async Task Arg_NullArgs_ReturnsFallback(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = null;

        // Act
        string result = TestDispatcher.TestArg(args, "name", "fallback");

        // Assert
        await Assert.That(result).IsEqualTo("fallback");
    }

    [Test]
    public async Task Arg_EmptyObject_ReturnsFallback(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{}""");

        // Act
        string result = TestDispatcher.TestArg(args, "name", "fallback");

        // Assert
        await Assert.That(result).IsEqualTo("fallback");
    }

    [Test]
    public async Task Arg_NullValue_ReturnsFallback(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"name": null}""");

        // Act
        string result = TestDispatcher.TestArg(args, "name", "fallback");

        // Assert
        await Assert.That(result).IsEqualTo("fallback");
    }

    [Test]
    public async Task Arg_ValidArgument_ReturnsParsedValue(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"name": "parsed"}""");

        // Act
        string result = TestDispatcher.TestArg(args, "name", "fallback");

        // Assert
        await Assert.That(result).IsEqualTo("parsed");
    }

    [Test]
    public async Task Arg_ValidIntArgument_ReturnsParsedValue(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"count": 99}""");

        // Act
        int result = TestDispatcher.TestArg(args, "count", 0);

        // Assert
        await Assert.That(result).IsEqualTo(99);
    }

    [Test]
    public async Task Arg_WrongType_ThrowsArgumentException(CancellationToken ct = default) {
        // Arrange
        JsonElement? args = ParseJson("""{"value": "not-a-number"}""");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            TestDispatcher.TestArg(args, "value", 0));

        await Assert.That(exception.Message).Contains("Argument 'value' is invalid.");
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static JsonElement? ParseJson(string json) {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private sealed class TestDispatcher : WindowFeatureWebMessageDispatcherBase<object> {
        public override string FeatureName => "test";

        protected override object SelectFeature(IInfiniFrameWindowFeatures features) => new object();

        public static T TestRequired<T>(JsonElement? args, string name) => Required<T>(args, name);
        public static T TestArg<T>(JsonElement? args, string name, T fallback) => Arg(args, name, fallback);
    }
}
