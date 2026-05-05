// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;
using System.Globalization;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GetWebMessageHandler {
    public static T RegisterGetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessageGetHandler(HandlerNames.GetRequest, HandleGetRequest);
        return builder;
    }

    private static string? HandleGetRequest(IInfiniFrameWindow window, string? payload) {
        // ReSharper disable once UnusedVariable
        if (!TryParseGetRequest(payload, out string? command, out JsonElement? args))
            return null;

        return command switch {
            "title" => window.Title,
            "width" => window.Width.ToString(CultureInfo.InvariantCulture),
            "height" => window.Height.ToString(CultureInfo.InvariantCulture),
            "left" => window.Left.ToString(CultureInfo.InvariantCulture),
            "top" => window.Top.ToString(CultureInfo.InvariantCulture),
            "maximized" => window.Maximized.ToString(),
            "minimized" => window.Minimized.ToString(),
            "fullscreen" => window.FullScreen.ToString(),
            "focused" => window.Focused.ToString(),
            "resizable" => window.Resizable.ToString(),
            "zoom" => window.Zoom.ToString(CultureInfo.InvariantCulture),
            _ => null
        };
    }

    private static bool TryParseGetRequest(string? payload, out string? command, out JsonElement? args) {
        command = null;
        args = null;

        if (string.IsNullOrWhiteSpace(payload))
            return false;

        try {
            using JsonDocument document = JsonDocument.Parse(payload);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return false;

            if (!document.RootElement.TryGetProperty("command", out JsonElement commandElement)
                || commandElement.ValueKind != JsonValueKind.String) {
                return false;
            }

            command = commandElement.GetString();
            if (string.IsNullOrWhiteSpace(command))
                return false;

            if (document.RootElement.TryGetProperty("args", out JsonElement argsElement))
                args = argsElement.Clone();

            return true;
        }
        catch (JsonException) {
            return false;
        }
    }
}
