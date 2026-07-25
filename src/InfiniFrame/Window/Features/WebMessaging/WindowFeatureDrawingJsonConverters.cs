// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class PointWebMessageJsonConverter : JsonConverter<Point> {
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement value = document.RootElement;
        return new Point(RequiredInt(value, "x"), RequiredInt(value, "y"));
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteEndObject();
    }

    internal static int RequiredInt(JsonElement value, string propertyName) {
        if (value.ValueKind != JsonValueKind.Object
            || !value.TryGetProperty(propertyName, out JsonElement property)
            || property.ValueKind != JsonValueKind.Number
            || !property.TryGetInt32(out int result)) {
            throw new JsonException($"Property '{propertyName}' must be a 32-bit integer.");
        }
        return result;
    }
}

internal sealed class SizeWebMessageJsonConverter : JsonConverter<Size> {
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement value = document.RootElement;
        return new Size(
            PointWebMessageJsonConverter.RequiredInt(value, "width"),
            PointWebMessageJsonConverter.RequiredInt(value, "height"));
    }

    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("width", value.Width);
        writer.WriteNumber("height", value.Height);
        writer.WriteEndObject();
    }
}

internal sealed class RectangleWebMessageJsonConverter : JsonConverter<Rectangle> {
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement value = document.RootElement;
        return new Rectangle(
            PointWebMessageJsonConverter.RequiredInt(value, "x"),
            PointWebMessageJsonConverter.RequiredInt(value, "y"),
            PointWebMessageJsonConverter.RequiredInt(value, "width"),
            PointWebMessageJsonConverter.RequiredInt(value, "height"));
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteNumber("width", value.Width);
        writer.WriteNumber("height", value.Height);
        writer.WriteEndObject();
    }
}
