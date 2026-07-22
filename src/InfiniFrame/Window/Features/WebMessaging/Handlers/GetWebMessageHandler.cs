// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GetWebMessageHandler {
    internal enum WindowFeatures {
        Browser,
        Debugging,
        Decorations,
        FilePickerDialogs,
        Invoke,
        Lifecycle,
        Monitors,
        Notifications,
        PageNavigation,
        Position,
        Size,
        State,
        WebMessaging
    }

    /// <summary>Registers the built-in JavaScript-to-window feature bridge.</summary>
    public static T RegisterGetWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessageGetHandler(JsHandlerNames.GetRequest, HandleGetRequest);
        builder.RegisterWebMessagePostHandler(JsHandlerNames.WindowFeatureRequest, HandlePostRequest);
        return builder;
    }

    private static string HandleGetRequest(IInfiniFrameWindow window, string? payload) {
        if (!TryParseFeatureRequest(payload, out WindowFeatures feature, out string command, out JsonElement? args))
            throw new ArgumentException("The window feature request is invalid.", nameof(payload));

        object? result = WindowFeatureWebMessageDispatcher.Get(window, feature, command, args);
        return WindowFeatureWebMessageDispatcher.Serialize(result);
    }

    private static void HandlePostRequest(IInfiniFrameWindow window, string? payload) {
        if (!TryParseFeatureRequest(payload, out WindowFeatures feature, out string command, out JsonElement? args))
            throw new ArgumentException("The window feature request is invalid.", nameof(payload));

        WindowFeatureWebMessageDispatcher.Post(window, feature, command, args);
    }

    private static bool TryParseFeatureRequest(
        string? payload,
        out WindowFeatures feature,
        out string command,
        out JsonElement? args
    ) {
        feature = default;
        command = string.Empty;
        args = null;

        return TryParseGetRequest(payload, out string? qualifiedCommand, out args)
            && TryParseCommandName(qualifiedCommand, out feature, out command);
    }

    internal static bool TryParseGetRequest(string? payload, out string? command, out JsonElement? args) {
        command = null;
        args = null;
        if (string.IsNullOrWhiteSpace(payload)) return false;

        try {
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("command", out JsonElement commandElement)
                || commandElement.ValueKind != JsonValueKind.String)
                return false;

            command = commandElement.GetString();
            if (root.TryGetProperty("args", out JsonElement argsElement)) args = argsElement.Clone();
            return !string.IsNullOrWhiteSpace(command);
        }
        catch (JsonException) {
            return false;
        }
    }

    internal static bool TryParseCommandName(
        string? command,
        out WindowFeatures feature,
        out string commandName
    ) {
        feature = default;
        commandName = string.Empty;
        if (string.IsNullOrWhiteSpace(command)) return false;

        string[] parts = command.Split(':');
        if (parts is not ["__infiniframe", "window", "features", {} featureName, {} member]
            || string.IsNullOrWhiteSpace(member)
            || !Enum.TryParse(featureName, true, out feature))
            return false;

        commandName = member;
        return true;
    }
}
