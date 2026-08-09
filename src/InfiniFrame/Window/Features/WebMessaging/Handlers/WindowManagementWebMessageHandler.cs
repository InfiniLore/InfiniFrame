// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame.Interop;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class WindowManagementWebMessageHandler {
    public static T RegisterWindowManagementWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMinimize,
            (window, _) => window.Features.State.SetMinimized());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowMaximize,
            (window, _) => window.Features.State.SetMaximized());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowClose,
            (window, _) => window.Features.Lifecycle.Close());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowToggleMaximize,
            (window, _) => window.Features.State.ToggleMaximized());

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowRestoreFromMaximized,
            (window, payload) => {
                window.Features.State.SetMaximized(false);
                if (string.IsNullOrEmpty(payload)) return;
                try {
                    using JsonDocument doc = JsonDocument.Parse(payload);
                    double screenX = doc.RootElement.GetProperty("screenX").GetDouble();
                    double screenY = doc.RootElement.GetProperty("screenY").GetDouble();
                    int halfWidth = window.Features.State.CachedPreMaximizedBounds.Width / 2;
                    window.Features.Position.Offset((int)(screenX - halfWidth), (int)(screenY - 10));
                } catch (JsonException) { /* Best effort positioning */ }
            });

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowOffsetPosition,
            (window, payload) => {
                if (string.IsNullOrEmpty(payload)) return;
                try {
                    using JsonDocument doc = JsonDocument.Parse(payload);
                    double left = doc.RootElement.GetProperty("left").GetDouble();
                    double top = doc.RootElement.GetProperty("top").GetDouble();
                    window.Features.Position.Offset(left, top);
                } catch (JsonException) { /* Best effort positioning */ }
            });

        builder.RegisterWebMessagePostHandler(
            JsHandlerNames.WindowResize,
            (window, payload) => {
                if (string.IsNullOrEmpty(payload)) return;
                try {
                    using JsonDocument doc = JsonDocument.Parse(payload);
                    int widthOffset = doc.RootElement.GetProperty("widthOffset").GetInt32();
                    int heightOffset = doc.RootElement.GetProperty("heightOffset").GetInt32();
                    ResizeOrigin origin = Enum.Parse<ResizeOrigin>(doc.RootElement.GetProperty("origin").GetString()!);
                    window.Features.Size.Resize(widthOffset, heightOffset, origin);
                } catch (JsonException) { /* Best effort resize */ }
                catch (ArgumentException) { /* Best effort resize */ }
            });

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, JsHandlerNames.RegisterWindowClose);
        return builder;
    }
}