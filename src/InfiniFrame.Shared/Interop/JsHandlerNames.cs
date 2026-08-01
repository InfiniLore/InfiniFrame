// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Contains constants for JavaScript handler names used in interop communication.
/// </summary>
public static class JsHandlerNames {
    private const string InfiniFramePrefix = "__infiniframe";

    internal const string FullscreenEnter = $"{InfiniFramePrefix}:fullscreen:enter";
    internal const string FullscreenExit = $"{InfiniFramePrefix}:fullscreen:exit";
    internal const string FullscreenToggle = $"{InfiniFramePrefix}:fullscreen:toggle";

    internal const string RegisterFullScreenChange = $"{InfiniFramePrefix}:register:fullscreen:change";

    internal const string OpenExternal = $"{InfiniFramePrefix}:open:external";
    internal const string RegisterOpenExternal = $"{InfiniFramePrefix}:register:open:external";

    internal const string TitleChanged = $"{InfiniFramePrefix}:title:change";
    internal const string RegisterTitleChange = $"{InfiniFramePrefix}:register:title:change";

    internal const string WindowReady = $"{InfiniFramePrefix}:ready";
    internal const string WindowReadyAck = $"{InfiniFramePrefix}:ready:ack";
    internal const string GetRequest = $"{InfiniFramePrefix}:get";
    internal const string GetResponse = $"{InfiniFramePrefix}:get:response";
    internal const string WebMessageAckRequest = $"{InfiniFramePrefix}:message:ack:request";
    internal const string WebMessageAckResponse = $"{InfiniFramePrefix}:message:ack:response";
    internal const string WindowFeatureRequest = $"{InfiniFramePrefix}:window:features";

    internal const string WindowMinimize = $"{InfiniFramePrefix}:window:minimize";
    internal const string WindowMaximize = $"{InfiniFramePrefix}:window:maximize";
    internal const string WindowClose = $"{InfiniFramePrefix}:window:close";

    internal const string RegisterWindowClose = $"{InfiniFramePrefix}:register:window:close";
}
