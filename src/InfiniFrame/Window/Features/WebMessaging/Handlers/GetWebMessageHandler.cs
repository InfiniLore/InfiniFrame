// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Globalization;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GetWebMessageHandler {
    public static T RegisterGetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessageGetHandler(JsHandlerNames.GetRequest, HandleGetRequest);
        return builder;
    }

    private static string? HandleGetRequest(IInfiniFrameWindow window, string? payload) {
        // ReSharper disable once UnusedVariable
        if (!TryParseGetRequest(payload, out string? command, out JsonElement? args))
            return null;

        return command switch {
            "title" => window.Features.Decorations.Title,
            "width" => window.Features.Size.Width.ToString(CultureInfo.InvariantCulture),
            "height" => window.Features.Size.Height.ToString(CultureInfo.InvariantCulture),
            "left" => window.Features.Position.Left.ToString(CultureInfo.InvariantCulture),
            "top" => window.Features.Position.Top.ToString(CultureInfo.InvariantCulture),
            "maximized" => window.Features.State.IsMaximized.ToString(),
            "minimized" => window.Features.State.IsMinimized.ToString(),
            "fullscreen" => window.Features.State.IsFullScreen.ToString(),
            "focused" => window.Features.State.IsFocused.ToString(),
            "resizable" => window.Features.Size.IsResizable.ToString(),
            "zoom" => window.Features.State.ZoomFactor.ToString(CultureInfo.InvariantCulture),
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
