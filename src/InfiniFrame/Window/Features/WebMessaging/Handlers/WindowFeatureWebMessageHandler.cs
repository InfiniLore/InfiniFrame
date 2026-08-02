// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class WindowFeatureWebMessageHandler {
    internal static T Register<T>(T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessageGetHandler(JsHandlerNames.GetRequest, HandleGetRequest);
        builder.RegisterWebMessagePostHandler(JsHandlerNames.WindowFeatureRequest, HandlePostRequest);
        return builder;
    }

    private static string HandleGetRequest(IInfiniFrameWindow window, string? payload) {
        WindowFeatureWebMessageRequest request = ParseRequest(payload);
        return WindowFeatureWebMessageRouter.Get(window, request.FeatureName, request.Command, request.Args);
    }

    private static void HandlePostRequest(IInfiniFrameWindow window, string? payload) {
        WindowFeatureWebMessageRequest request = ParseRequest(payload);
        WindowFeatureWebMessageRouter.Post(window, request.FeatureName, request.Command, request.Args);
    }

    private static WindowFeatureWebMessageRequest ParseRequest(string? payload) {
        if (!TryParseRequest(payload, out WindowFeatureWebMessageRequest request))
            throw new ArgumentException("The window feature request is invalid.", nameof(payload));
        return request;
    }

    internal static bool TryParseRequest(string? payload, out WindowFeatureWebMessageRequest request) {
        request = default;
        if (string.IsNullOrWhiteSpace(payload)) return false;

        try {
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("command", out JsonElement commandElement)
                || commandElement.ValueKind != JsonValueKind.String)
                return false;

            string? qualifiedCommand = commandElement.GetString();
            if (!TryParseCommandName(qualifiedCommand, out string featureName, out string command)) return false;

            JsonElement? args = root.TryGetProperty("args", out JsonElement argsElement)
                ? argsElement.Clone()
                : null;
            request = new WindowFeatureWebMessageRequest(featureName, command, args);
            return true;
        }
        catch (JsonException) {
            return false;
        }
    }

    private static bool TryParseCommandName(string? qualifiedCommand, out string featureName, out string command) {
        featureName = string.Empty;
        command = string.Empty;
        if (string.IsNullOrWhiteSpace(qualifiedCommand)) return false;

        if (qualifiedCommand.Split(':') is not ["__infiniframe", "window", "features", { } parsedFeature, { } parsedCommand]
            || string.IsNullOrWhiteSpace(parsedFeature)
            || string.IsNullOrWhiteSpace(parsedCommand))
            return false;

        featureName = parsedFeature;
        command = parsedCommand;
        return true;
    }
}

internal readonly record struct WindowFeatureWebMessageRequest(
    string FeatureName,
    string Command,
    JsonElement? Args
);