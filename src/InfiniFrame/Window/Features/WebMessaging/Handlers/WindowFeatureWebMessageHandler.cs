// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Dialogs;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class WindowFeatureWebMessageDispatcher {
    internal static string Serialize(object? value) {
        if (value is null) return "null";

        JsonTypeInfo typeInfo = WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(value.GetType())
            ?? throw new InvalidOperationException($"No JSON metadata is registered for '{value.GetType()}'.");
        return JsonSerializer.Serialize(value, typeInfo);
    }

    internal static object? Get(
        IInfiniFrameWindow window,
        GetWebMessageHandler.WindowFeatures feature,
        string command,
        JsonElement? args
    ) => feature switch {
        GetWebMessageHandler.WindowFeatures.Browser => GetBrowser(window.Features.Browser, command),
        GetWebMessageHandler.WindowFeatures.Debugging => GetDebugging(window.Features.Debugging, command),
        GetWebMessageHandler.WindowFeatures.Decorations => GetDecorations(window.Features.Decorations, command),
        GetWebMessageHandler.WindowFeatures.FilePickerDialogs => GetFilePickerDialogs(window.Features.FilePickerDialogs, command, args),
        GetWebMessageHandler.WindowFeatures.Lifecycle => GetLifecycle(window.Features.Lifecycle, command),
        GetWebMessageHandler.WindowFeatures.Monitors => GetMonitors(window.Features.Monitors, command),
        GetWebMessageHandler.WindowFeatures.Notifications => GetNotifications(window.Features.Notifications, command, args),
        GetWebMessageHandler.WindowFeatures.PageNavigation => GetPageNavigation(window.Features.PageNavigation, command, args),
        GetWebMessageHandler.WindowFeatures.Position => GetPosition(window.Features.Position, command),
        GetWebMessageHandler.WindowFeatures.Size => GetSize(window.Features.Size, command),
        GetWebMessageHandler.WindowFeatures.State => GetState(window.Features.State, command),
        GetWebMessageHandler.WindowFeatures.Invoke => throw Unsupported(feature, command),
        GetWebMessageHandler.WindowFeatures.WebMessaging => throw Unsupported(feature, command),
        _ => throw Unsupported(feature, command)
    };

    internal static void Post(
        IInfiniFrameWindow window,
        GetWebMessageHandler.WindowFeatures feature,
        string command,
        JsonElement? args
    ) {
        switch (feature) {
            case GetWebMessageHandler.WindowFeatures.Browser:
                PostBrowser(window.Features.Browser, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.Debugging:
                PostDebugging(window.Features.Debugging, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.Decorations:
                PostDecorations(window.Features.Decorations, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.Lifecycle:
                PostLifecycle(window.Features.Lifecycle, command);
                return;
            case GetWebMessageHandler.WindowFeatures.Notifications:
                PostNotifications(window.Features.Notifications, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.PageNavigation:
                PostPageNavigation(window.Features.PageNavigation, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.Position:
                PostPosition(window.Features.Position, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.Size:
                PostSize(window.Features.Size, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.State:
                PostState(window.Features.State, command, args);
                return;
            case GetWebMessageHandler.WindowFeatures.WebMessaging:
                PostWebMessaging(window.Features.WebMessaging, command, args);
                return;
            
            case GetWebMessageHandler.WindowFeatures.FilePickerDialogs:
            case GetWebMessageHandler.WindowFeatures.Invoke:
            case GetWebMessageHandler.WindowFeatures.Monitors:
            default: throw Unsupported(feature, command);
        }
    }

    private static object? GetBrowser(IInfiniFrameWindowFeatureBrowser feature, string command) => command switch {
        "isContextMenuEnabled" => feature.IsContextMenuEnabled,
        "isMediaAutoplayEnabled" => feature.IsMediaAutoplayEnabled,
        "userAgent" => feature.UserAgent,
        "isFileSystemAccessEnabled" => feature.IsFileSystemAccessEnabled,
        "isWebSecurityEnabled" => feature.IsWebSecurityEnabled,
        "isJavascriptClipboardAccessEnabled" => feature.IsJavascriptClipboardAccessEnabled,
        "isMediaStreamEnabled" => feature.IsMediaStreamEnabled,
        "isIgnoreCertificateErrorsEnabled" => feature.IsIgnoreCertificateErrorsEnabled,
        "grantBrowserPermissions" => feature.GrantBrowserPermissions,
        "isSmoothScrollingEnabled" => feature.IsSmoothScrollingEnabled,
        "browserControlInitParameters" => feature.BrowserControlInitParameters,
        _ => throw Unsupported("browser", command)
    };

    private static void PostBrowser(IInfiniFrameWindowFeatureBrowser feature, string command, JsonElement? args) {
        switch (command) {
            case "enableContextMenu":
                feature.EnableContextMenu(Arg(args, "enabled", true));
                return;
            case "enableMediaAutoplay":
                feature.EnableMediaAutoplay(Arg(args, "enabled", true));
                return;
            case "setUserAgent":
                feature.SetUserAgent(Arg<string?>(args, "userAgent", null));
                return;
            case "win32SetWebView2Path":
                feature.Win32SetWebView2Path(Required<string>(args, "path"));
                return;
            case "clearBrowserAutoFill":
                feature.ClearBrowserAutoFill();
                return;
            default: throw Unsupported("browser", command);
        }
    }

    private static object? GetDebugging(IInfiniFrameWindowFeatureDebugging feature, string command) => command switch {
        "isDevToolsEnabled" => feature.IsDevToolsEnabled,
        "supportsWebInspectorAttach" => feature.SupportsWebInspectorAttach,
        "isWebInspectorEnabled" => feature.IsWebInspectorEnabled,
        "supportsRemoteDebuggingEndpoint" => feature.SupportsRemoteDebuggingEndpoint,
        "remoteDebuggingPort" => feature.RemoteDebuggingPort,
        "capabilities" => feature.Capabilities,
        "diagnostics" => feature.GetDiagnostics(),
        "remoteDebuggingEndpoint" => GetRemoteDebuggingEndpoint(feature),
        "probeEndpoint" => ProbeEndpoint(feature),
        _ => throw Unsupported("debugging", command)
    };

    private static void PostDebugging(IInfiniFrameWindowFeatureDebugging feature, string command, JsonElement? args) {
        if (command == "enableDevTools") feature.EnableDevTools(Required<bool>(args, "enabled"));
        else throw Unsupported("debugging", command);
    }

    private static DebugEndpointResult GetRemoteDebuggingEndpoint(IInfiniFrameWindowFeatureDebugging feature) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            return new DebugEndpointResult(false, null, "Remote debugging endpoints are not supported on this platform.");

        bool success = feature.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
        return new DebugEndpointResult(success, endpoint?.ToString(), null);
    }

    private static DebugEndpointResult ProbeEndpoint(IInfiniFrameWindowFeatureDebugging feature) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            return new DebugEndpointResult(false, null, "Remote debugging endpoints are not supported on this platform.");

        bool success = feature.TryProbeEndpoint(out Uri? endpoint, out string? reason);
        return new DebugEndpointResult(success, endpoint?.ToString(), reason);
    }

    private static object? GetDecorations(IInfiniFrameWindowFeatureDecorations feature, string command) => command switch {
        "isChromeless" => feature.IsChromeless,
        "isTransparent" => feature.IsTransparent,
        "title" or "getTitle" => feature.Title,
        "iconFilePath" => feature.IconFilePath,
        "limitLinuxWindowTitleLength" => feature.LimitLinuxWindowTitleLength,
        _ => throw Unsupported("decorations", command)
    };

    private static void PostDecorations(IInfiniFrameWindowFeatureDecorations feature, string command, JsonElement? args) {
        switch (command) {
            case "setTransparent":
                feature.SetTransparent(Arg(args, "enabled", true));
                return;
            case "setTitle":
                feature.SetTitle(Arg<string?>(args, "title", null));
                return;
            case "setIconFile":
                feature.SetIconFile(Required<string>(args, "iconFilePath"));
                return;
            case "setLimitLinuxWindowTitleLength":
                feature.SetLimitLinuxWindowTitleLength(Arg(args, "enabled", true));
                return;
            default: throw Unsupported("decorations", command);
        }
    }

    private static object? GetFilePickerDialogs(IInfiniFrameWindowFeatureFilePickerDialogs feature, string command, JsonElement? args) {
        string? defaultPath = Arg<string?>(args, "defaultPath", null);
        WindowFeatureFilePickerFilter[]? filterDtos = Arg<WindowFeatureFilePickerFilter[]?>(args, "filters", null);
        (string Name, string[] Extensions)[]? filters = filterDtos?.Select(f => (f.Name, f.Extensions)).ToArray();
        return command switch {
            "showOpenFile" => feature.ShowOpenFile(
                Arg(args, "title", "Choose file"), defaultPath, Arg(args, "multiSelect", false), filters),
            "showOpenFolder" => feature.ShowOpenFolder(
                Arg(args, "title", "Select folder"), defaultPath, Arg(args, "multiSelect", false)),
            "showSaveFile" => feature.ShowSaveFile(
                Arg(args, "title", "Save file"), defaultPath, filters),
            _ => throw Unsupported("filePickerDialogs", command)
        };
    }

    private static object GetLifecycle(IInfiniFrameWindowFeatureLifecycle feature, string command) => command switch {
        "state" => feature.State,
        "isClosedOrClosing" => feature.IsClosedOrClosing(),
        _ => throw Unsupported("lifecycle", command)
    };

    private static void PostLifecycle(IInfiniFrameWindowFeatureLifecycle feature, string command) {
        if (command == "close") feature.Close();
        else throw Unsupported("lifecycle", command);
    }

    private static object GetMonitors(IInfiniFrameWindowFeatureMonitors feature, string command) => command switch {
        "monitors" => feature.GetMonitors().ToArray(),
        "mainMonitor" => feature.GetMainMonitor(),
        "mainMonitorScreenDpi" => feature.GetMainMonitorScreenDpi(),
        _ => throw Unsupported("monitors", command)
    };

    private static object GetNotifications(IInfiniFrameWindowFeatureNotifications feature, string command, JsonElement? args)
        => command == "showMessage"
            ? feature.ShowMessage(
                Required<string>(args, "title"),
                Arg<string?>(args, "text", null),
                Arg(args, "buttons", InfiniFrameDialogButtons.Ok),
                Arg(args, "icon", InfiniFrameDialogIcon.Info))
            : throw Unsupported("notifications", command);

    private static void PostNotifications(IInfiniFrameWindowFeatureNotifications feature, string command, JsonElement? args) {
        if (command == "showNotification")
            feature.ShowNotification(Required<string>(args, "title"), Required<string>(args, "body"));
        else throw Unsupported("notifications", command);
    }

    private static object GetPageNavigation(IInfiniFrameWindowFeaturePageNavigation feature, string command, JsonElement? args) => command switch {
        "tryLoadUri" => feature.TryLoadUri(new Uri(Required<string>(args, "uri"), UriKind.RelativeOrAbsolute)),
        "tryLoadPath" => feature.TryLoadPath(Required<string>(args, "path")),
        _ => throw Unsupported("pageNavigation", command)
    };

    private static void PostPageNavigation(IInfiniFrameWindowFeaturePageNavigation feature, string command, JsonElement? args) {
        switch (command) {
            case "loadUri":
                feature.Load(new Uri(Required<string>(args, "uri"), UriKind.RelativeOrAbsolute));
                return;
            case "loadPath":
                feature.Load(Required<string>(args, "path"));
                return;
            case "loadRawString":
                feature.LoadRawString(Required<string>(args, "content"));
                return;
            default: throw Unsupported("pageNavigation", command);
        }
    }

    private static object GetPosition(IInfiniFrameWindowFeaturePosition feature, string command) => command switch {
        "location" => feature.Location,
        "top" => feature.Top,
        "left" => feature.Left,
        _ => throw Unsupported("position", command)
    };

    private static void PostPosition(IInfiniFrameWindowFeaturePosition feature, string command, JsonElement? args) {
        switch (command) {
            case "setLocation":
                feature.SetLocation(Required<int>(args, "left"), Required<int>(args, "top"));
                return;
            case "setLeft":
                feature.SetLeft(Required<int>(args, "left"));
                return;
            case "setTop":
                feature.SetTop(Required<int>(args, "top"));
                return;
            case "offset":
                feature.Offset(Required<double>(args, "left"), Required<double>(args, "top"));
                return;
            case "center":
                feature.Center();
                return;
            case "centerOnCurrentMonitor":
                feature.CenterOnCurrentMonitor();
                return;
            case "centerOnMonitor":
                feature.CenterOnMonitor(Required<int>(args, "monitorIndex"));
                return;
            case "moveWithinCurrentMonitorArea":
                feature.MoveWithinCurrentMonitorArea(
                    Required<double>(args, "left"), Required<double>(args, "top"));
                return;
            default: throw Unsupported("position", command);
        }
    }

    private static object GetSize(IInfiniFrameWindowFeatureSize feature, string command) => command switch {
        "size" => feature.Size,
        "height" => feature.Height,
        "width" => feature.Width,
        "maxSize" => feature.MaxSize,
        "maxHeight" => feature.MaxHeight,
        "maxWidth" => feature.MaxWidth,
        "minSize" => feature.MinSize,
        "minHeight" => feature.MinHeight,
        "minWidth" => feature.MinWidth,
        "isResizable" => feature.IsResizable,
        _ => throw Unsupported("size", command)
    };

    private static void PostSize(IInfiniFrameWindowFeatureSize feature, string command, JsonElement? args) {
        switch (command) {
            case "setSize":
                feature.SetSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setHeight":
                feature.SetHeight(Required<int>(args, "height"));
                return;
            case "setMaxSize":
                feature.SetMaxSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setMaxHeight":
                feature.SetMaxHeight(Required<int>(args, "height"));
                return;
            case "setMaxWidth":
                feature.SetMaxWidth(Required<int>(args, "width"));
                return;
            case "setMinSize":
                feature.SetMinSize(Required<int>(args, "width"), Required<int>(args, "height"));
                return;
            case "setMinHeight":
                feature.SetMinHeight(Required<int>(args, "height"));
                return;
            case "setMinWidth":
                feature.SetMinWidth(Required<int>(args, "width"));
                return;
            case "setWidth":
                feature.SetWidth(Required<int>(args, "width"));
                return;
            case "resize":
                feature.Resize(
                    Required<int>(args, "widthOffset"), Required<int>(args, "heightOffset"), Required<ResizeOrigin>(args, "origin"));
                return;
            case "setResizable":
                feature.SetResizable(Arg(args, "resizable", true));
                return;
            default: throw Unsupported("size", command);
        }
    }

    private static object GetState(IInfiniFrameWindowFeatureState feature, string command) => command switch {
        "isFullScreen" => feature.IsFullScreen,
        "isMaximized" => feature.IsMaximized,
        "isMinimized" => feature.IsMinimized,
        "isTopMost" => feature.IsTopMost,
        "isFocused" => feature.IsFocused,
        "zoomFactor" => feature.ZoomFactor,
        "isZoomEnabled" => feature.IsZoomEnabled,
        "cachedPreFullScreenBounds" => feature.CachedPreFullScreenBounds,
        "cachedPreMaximizedBounds" => feature.CachedPreMaximizedBounds,
        _ => throw Unsupported("state", command)
    };

    private static void PostState(IInfiniFrameWindowFeatureState feature, string command, JsonElement? args) {
        switch (command) {
            case "setMaximized":
                feature.SetMaximized(Arg(args, "maximized", true));
                return;
            case "toggleMaximized":
                feature.ToggleMaximized();
                return;
            case "setMinimized":
                feature.SetMinimized(Arg(args, "minimized", true));
                return;
            case "setFullScreen":
                feature.SetFullScreen(Arg(args, "fullScreen", true));
                return;
            case "setFocused":
                feature.SetFocused();
                return;
            case "setZoomFactor":
                feature.SetZoomFactor(Required<int>(args, "zoom"));
                return;
            case "enableZoom":
                feature.EnableZoom(Arg(args, "enabled", true));
                return;
            case "setTopMost":
                feature.SetTopMost(Arg(args, "topMost", true));
                return;
            default: throw Unsupported("state", command);
        }
    }

    private static void PostWebMessaging(IInfiniFrameWindowFeatureWebMessaging feature, string command, JsonElement? args) {
        if (command == "sendWebMessage") feature.SendWebMessage(Required<string>(args, "message"));
        else throw Unsupported("webMessaging", command);
    }

    private static T Required<T>(JsonElement? args, string name) {
        if (args is not { ValueKind: JsonValueKind.Object } value || !value.TryGetProperty(name, out JsonElement property))
            throw new ArgumentException($"Argument '{name}' is required.");

        JsonTypeInfo typeInfo = WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(typeof(T))
            ?? throw new InvalidOperationException($"No JSON metadata is registered for '{typeof(T)}'.");
        return (T?)property.Deserialize(typeInfo)
            ?? throw new ArgumentException($"Argument '{name}' cannot be null.");
    }

    private static T Arg<T>(JsonElement? args, string name, T fallback) {
        if (args is not { ValueKind: JsonValueKind.Object } value || !value.TryGetProperty(name, out JsonElement property))
            return fallback;
        if (property.ValueKind == JsonValueKind.Null) return fallback;

        JsonTypeInfo typeInfo = WindowFeatureWebMessageJsonContext.Default.GetTypeInfo(typeof(T))
            ?? throw new InvalidOperationException($"No JSON metadata is registered for '{typeof(T)}'.");
        return (T?)property.Deserialize(typeInfo) ?? fallback;
    }

    private static InvalidOperationException Unsupported(object feature, string command)
        => new($"Window feature command '{feature}:{command}' is not supported.");

}

internal sealed record WindowFeatureFilePickerFilter(string Name, string[] Extensions);

internal sealed record DebugEndpointResult(bool Success, string? Endpoint, string? Reason);

[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(InfiniMonitor))]
[JsonSerializable(typeof(InfiniMonitor[]))]
[JsonSerializable(typeof(Point))]
[JsonSerializable(typeof(Size))]
[JsonSerializable(typeof(Rectangle))]
[JsonSerializable(typeof(InfiniFrameDebugCapabilities))]
[JsonSerializable(typeof(InfiniFrameDebugDiagnostics))]
[JsonSerializable(typeof(DebugEndpointResult))]
[JsonSerializable(typeof(WindowFeatureFilePickerFilter[]))]
[JsonSerializable(typeof(ResizeOrigin))]
[JsonSerializable(typeof(InfiniFrameDialogButtons))]
[JsonSerializable(typeof(InfiniFrameDialogIcon))]
[JsonSerializable(typeof(InfiniFrameDialogResult))]
[JsonSerializable(typeof(InfiniFrameWindowLifecycleState))]
internal partial class WindowFeatureWebMessageJsonContext : JsonSerializerContext;
