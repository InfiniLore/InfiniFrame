// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WebMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureDrawingJsonConverterTests {

    private static JsonSerializerOptions CreateOptions() {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new PointWebMessageJsonConverter());
        options.Converters.Add(new SizeWebMessageJsonConverter());
        options.Converters.Add(new RectangleWebMessageJsonConverter());
        return options;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Point Converter
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Point_RoundTrip(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        var point = new Point(100, 200);

        // Act
        string json = JsonSerializer.Serialize(point, options);
        Point deserialized = JsonSerializer.Deserialize<Point>(json, options);

        // Assert
        await Assert.That(deserialized.X).IsEqualTo(100);
        await Assert.That(deserialized.Y).IsEqualTo(200);
    }

    [Test]
    public async Task Point_MissingX_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"y": 10}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Point>(json, options))
            .Throws<JsonException>();
    }

    [Test]
    public async Task Point_MissingY_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"x": 10}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Point>(json, options))
            .Throws<JsonException>();
    }

    [Test]
    public async Task Point_WrongType_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"x": "not-a-number", "y": 10}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Point>(json, options))
            .Throws<JsonException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Size Converter
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Size_RoundTrip(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        var size = new Size(800, 600);

        // Act
        string json = JsonSerializer.Serialize(size, options);
        Size deserialized = JsonSerializer.Deserialize<Size>(json, options);

        // Assert
        await Assert.That(deserialized.Width).IsEqualTo(800);
        await Assert.That(deserialized.Height).IsEqualTo(600);
    }

    [Test]
    public async Task Size_MissingWidth_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"height": 10}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Size>(json, options))
            .Throws<JsonException>();
    }

    [Test]
    public async Task Size_MissingHeight_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"width": 10}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Size>(json, options))
            .Throws<JsonException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Rectangle Converter
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task Rectangle_RoundTrip(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        var rect = new Rectangle(10, 20, 300, 400);

        // Act
        string json = JsonSerializer.Serialize(rect, options);
        Rectangle deserialized = JsonSerializer.Deserialize<Rectangle>(json, options);

        // Assert
        await Assert.That(deserialized.X).IsEqualTo(10);
        await Assert.That(deserialized.Y).IsEqualTo(20);
        await Assert.That(deserialized.Width).IsEqualTo(300);
        await Assert.That(deserialized.Height).IsEqualTo(400);
    }

    [Test]
    public async Task Rectangle_MissingProperty_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        JsonSerializerOptions options = CreateOptions();
        string json = """{"x": 0, "y": 0, "width": 100}""";

        // Act & Assert
        await Assert.That(() => JsonSerializer.Deserialize<Rectangle>(json, options))
            .Throws<JsonException>();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // RequiredInt helper
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task RequiredInt_NonObjectValue_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        string json = """42""";
        using JsonDocument doc = JsonDocument.Parse(json);

        // Act & Assert
        await Assert.That(() => PointWebMessageJsonConverter.RequiredInt(doc.RootElement, "x"))
            .Throws<JsonException>();
    }

    [Test]
    public async Task RequiredInt_MissingProperty_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        string json = """{"y": 10}""";
        using JsonDocument doc = JsonDocument.Parse(json);

        // Act & Assert
        await Assert.That(() => PointWebMessageJsonConverter.RequiredInt(doc.RootElement, "x"))
            .Throws<JsonException>();
    }

    [Test]
    public async Task RequiredInt_NonIntegerValue_ThrowsJsonException(CancellationToken ct = default) {
        // Arrange
        string json = """{"x": "hello"}""";
        using JsonDocument doc = JsonDocument.Parse(json);

        // Act & Assert
        await Assert.That(() => PointWebMessageJsonConverter.RequiredInt(doc.RootElement, "x"))
            .Throws<JsonException>();
    }
}
