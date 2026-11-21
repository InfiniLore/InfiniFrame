// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Hosting;
using InfiniFrame.Js.Interop;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Security;
using InfiniFrame.Window.Builder;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Maps a window builder to the public <see cref="InfiniFrameHostWindowRequest" />. Every native window-parameter
///     field is classified deliberately: mapped into the request, rejected unless left at its default, or owned by core.
///     Nothing is forwarded by blindly calling default-engine setters, and nothing unsupported is silently ignored.
/// </summary>
internal static class InfiniFrameHostedWindowRequestFactory {
    internal const int MaxIconBytes = 1024 * 1024;
    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];
    private static readonly Lazy<InfiniFrameNativeWindowParameters> Defaults = new(
        static () => new InfiniFrameWindowBuilder().CollectNativeParameters(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Fields carried by <see cref="InfiniFrameHostWindowRequest" /> (directly or through its browser policy).</summary>
    internal static readonly IReadOnlySet<string> MappedFields = new HashSet<string>(StringComparer.Ordinal) {
        nameof(InfiniFrameNativeWindowParameters.StartUrl), nameof(InfiniFrameNativeWindowParameters.Title),
        nameof(InfiniFrameNativeWindowParameters.WindowIconFile), nameof(InfiniFrameNativeWindowParameters.UserAgent),
        nameof(InfiniFrameNativeWindowParameters.Left), nameof(InfiniFrameNativeWindowParameters.Top),
        nameof(InfiniFrameNativeWindowParameters.Width), nameof(InfiniFrameNativeWindowParameters.Height),
        nameof(InfiniFrameNativeWindowParameters.UseOsDefaultLocation), nameof(InfiniFrameNativeWindowParameters.UseOsDefaultSize),
        nameof(InfiniFrameNativeWindowParameters.FullScreen), nameof(InfiniFrameNativeWindowParameters.Maximized),
        nameof(InfiniFrameNativeWindowParameters.Minimized), nameof(InfiniFrameNativeWindowParameters.ContextMenuEnabled),
        nameof(InfiniFrameNativeWindowParameters.DevToolsEnabled), nameof(InfiniFrameNativeWindowParameters.GrantBrowserPermissions),
        nameof(InfiniFrameNativeWindowParameters.MediaAutoplayEnabled), nameof(InfiniFrameNativeWindowParameters.FileSystemAccessEnabled),
        nameof(InfiniFrameNativeWindowParameters.WebSecurityEnabled), nameof(InfiniFrameNativeWindowParameters.JavascriptClipboardAccessEnabled),
        nameof(InfiniFrameNativeWindowParameters.MediaStreamEnabled), nameof(InfiniFrameNativeWindowParameters.IgnoreCertificateErrorsEnabled)
    };

    /// <summary>Default-engine settings with no host-contract equivalent: accepted only at their default value.</summary>
    internal static readonly IReadOnlySet<string> RejectedUnlessDefaultFields = new HashSet<string>(StringComparer.Ordinal) {
        nameof(InfiniFrameNativeWindowParameters.StartString), nameof(InfiniFrameNativeWindowParameters.TemporaryFilesPath),
        nameof(InfiniFrameNativeWindowParameters.BrowserControlInitParameters), nameof(InfiniFrameNativeWindowParameters.DefaultNotificationIcon),
        nameof(InfiniFrameNativeWindowParameters.RemoteDebuggingPort), nameof(InfiniFrameNativeWindowParameters.NativeParent),
        nameof(InfiniFrameNativeWindowParameters.DragDropEnabled), nameof(InfiniFrameNativeWindowParameters.Zoom),
        nameof(InfiniFrameNativeWindowParameters.MinWidth), nameof(InfiniFrameNativeWindowParameters.MinHeight),
        nameof(InfiniFrameNativeWindowParameters.MaxWidth), nameof(InfiniFrameNativeWindowParameters.MaxHeight),
        nameof(InfiniFrameNativeWindowParameters.CenterOnInitialize), nameof(InfiniFrameNativeWindowParameters.Chromeless),
        nameof(InfiniFrameNativeWindowParameters.Transparent), nameof(InfiniFrameNativeWindowParameters.ZoomEnabled),
        nameof(InfiniFrameNativeWindowParameters.WebInspectorEnabled), nameof(InfiniFrameNativeWindowParameters.Resizable),
        nameof(InfiniFrameNativeWindowParameters.Topmost), nameof(InfiniFrameNativeWindowParameters.SmoothScrollingEnabled),
        nameof(InfiniFrameNativeWindowParameters.StatusBarEnabled), nameof(InfiniFrameNativeWindowParameters.BrowserShortcutsEnabled),
        nameof(InfiniFrameNativeWindowParameters.BackgroundColorR), nameof(InfiniFrameNativeWindowParameters.BackgroundColorG),
        nameof(InfiniFrameNativeWindowParameters.BackgroundColorB), nameof(InfiniFrameNativeWindowParameters.BackgroundColorA),
        nameof(InfiniFrameNativeWindowParameters.MenuBarJson)
    };

    /// <summary>
    ///     Fields owned by core or unreachable on hosted windows: native callback pointers and scheme-name memory (the
    ///     provider receives events and schemes through the contract), the native application handle, the struct size,
    ///     and the notification switch (hosted windows expose no notification feature, so either value is truthful).
    /// </summary>
    internal static readonly IReadOnlySet<string> CoreOwnedFields = new HashSet<string>(StringComparer.Ordinal) {
        nameof(InfiniFrameNativeWindowParameters.ApplicationInstance), nameof(InfiniFrameNativeWindowParameters.ClosingHandler),
        nameof(InfiniFrameNativeWindowParameters.ClosedHandler), nameof(InfiniFrameNativeWindowParameters.FocusInHandler),
        nameof(InfiniFrameNativeWindowParameters.FocusOutHandler), nameof(InfiniFrameNativeWindowParameters.ResizedHandler),
        nameof(InfiniFrameNativeWindowParameters.MaximizedHandler), nameof(InfiniFrameNativeWindowParameters.RestoredHandler),
        nameof(InfiniFrameNativeWindowParameters.MinimizedHandler), nameof(InfiniFrameNativeWindowParameters.MovedHandler),
        nameof(InfiniFrameNativeWindowParameters.WebMessageReceivedHandler), nameof(InfiniFrameNativeWindowParameters.DebugEventHandler),
        nameof(InfiniFrameNativeWindowParameters.CustomSchemeNames), nameof(InfiniFrameNativeWindowParameters.CustomSchemeHandler),
        nameof(InfiniFrameNativeWindowParameters.NavigationStartingHandler), nameof(InfiniFrameNativeWindowParameters.FileDroppedHandler),
        nameof(InfiniFrameNativeWindowParameters.NotificationsEnabled), nameof(InfiniFrameNativeWindowParameters.Size)
    };

    internal static InfiniFrameHostWindowRequest Create(
        InfiniFrameWindowBuilder builder,
        Guid windowId,
        InfiniFrameHostCapabilities capabilities,
        string providerName,
        out InfiniFrameNativeWindowParameters parameters
    ) {
        ArgumentNullException.ThrowIfNull(builder);
        parameters = builder.CollectNativeParameters();
        InfiniFrameNativeWindowParameters defaults = Defaults.Value;
        var violations = new List<string>();

        if (builder.Configuration.ParentWindow is not null) violations.Add("parent windows");
        RejectChanged(violations, "StartString (raw HTML start page; serve content through a custom scheme)", parameters.StartString, defaults.StartString);
        RejectChanged(violations, nameof(parameters.TemporaryFilesPath), parameters.TemporaryFilesPath, defaults.TemporaryFilesPath);
        RejectChanged(violations, nameof(parameters.BrowserControlInitParameters), parameters.BrowserControlInitParameters, defaults.BrowserControlInitParameters);
        RejectChanged(violations, nameof(parameters.DefaultNotificationIcon), parameters.DefaultNotificationIcon, defaults.DefaultNotificationIcon);
        RejectChanged(violations, nameof(parameters.RemoteDebuggingPort), parameters.RemoteDebuggingPort, defaults.RemoteDebuggingPort);
        RejectChanged(violations, nameof(parameters.NativeParent), parameters.NativeParent, defaults.NativeParent);
        RejectChanged(violations, nameof(parameters.DragDropEnabled), parameters.DragDropEnabled, defaults.DragDropEnabled);
        RejectChanged(violations, nameof(parameters.Zoom), parameters.Zoom, defaults.Zoom);
        RejectChanged(violations, nameof(parameters.ZoomEnabled), parameters.ZoomEnabled, defaults.ZoomEnabled);
        RejectChanged(violations, nameof(parameters.MinWidth), parameters.MinWidth, defaults.MinWidth);
        RejectChanged(violations, nameof(parameters.MinHeight), parameters.MinHeight, defaults.MinHeight);
        RejectChanged(violations, nameof(parameters.MaxWidth), parameters.MaxWidth, defaults.MaxWidth);
        RejectChanged(violations, nameof(parameters.MaxHeight), parameters.MaxHeight, defaults.MaxHeight);
        RejectChanged(violations, "CenterOnInitialize (monitor-relative placement)", parameters.CenterOnInitialize, defaults.CenterOnInitialize);
        RejectChanged(violations, nameof(parameters.Chromeless), parameters.Chromeless, defaults.Chromeless);
        RejectChanged(violations, nameof(parameters.Transparent), parameters.Transparent, defaults.Transparent);
        RejectChanged(violations, nameof(parameters.WebInspectorEnabled), parameters.WebInspectorEnabled, defaults.WebInspectorEnabled);
        RejectChanged(violations, nameof(parameters.Resizable), parameters.Resizable, defaults.Resizable);
        RejectChanged(violations, nameof(parameters.Topmost), parameters.Topmost, defaults.Topmost);
        RejectChanged(violations, nameof(parameters.SmoothScrollingEnabled), parameters.SmoothScrollingEnabled, defaults.SmoothScrollingEnabled);
        RejectChanged(violations, nameof(parameters.StatusBarEnabled), parameters.StatusBarEnabled, defaults.StatusBarEnabled);
        RejectChanged(violations, nameof(parameters.BrowserShortcutsEnabled), parameters.BrowserShortcutsEnabled, defaults.BrowserShortcutsEnabled);
        if (parameters.BackgroundColorR != defaults.BackgroundColorR || parameters.BackgroundColorG != defaults.BackgroundColorG
            || parameters.BackgroundColorB != defaults.BackgroundColorB || parameters.BackgroundColorA != defaults.BackgroundColorA)
            violations.Add("BackgroundColor");
        RejectChanged(violations, "MenuBarJson (native menus)", parameters.MenuBarJson, defaults.MenuBarJson);

        IInfiniFrameEventsStore events = builder.EventsStore;
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.BoundsEvents, "WindowLocationChanged", events.WindowLocationChanged.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.BoundsEvents, "WindowSizeChanged", events.WindowSizeChanged.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.StateEvents, "WindowMaximized", events.WindowMaximized.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.StateEvents, "WindowMinimized", events.WindowMinimized.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.StateEvents, "WindowRestored", events.WindowRestored.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.FocusEvents, "WindowFocusIn", events.WindowFocusIn.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.FocusEvents, "WindowFocusOut", events.WindowFocusOut.Snapshot.Length);
        RequireForHandlers(violations, capabilities, InfiniFrameHostCapabilities.CloseCancellation, "Closing (cancellable close)", events.Closing.Snapshot.Length);
        if (events.NavigationStarting.Snapshot.Length != 0) violations.Add("NavigationStarting handlers (not part of host contract v1)");
        if (events.FileDropped.Snapshot.Length != 0) violations.Add("FileDropped handlers (drag and drop)");
        if (events.DebuggingEvent.Snapshot.Length != 0) violations.Add("debugging event handlers");

        IInfiniFrameUriSecurityPolicy policy = InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(builder);
        Uri? startUrl = null;
        if (string.IsNullOrWhiteSpace(parameters.StartUrl)) violations.Add("a start URL is required (SetStartPageUrl)");
        else if (!Uri.TryCreate(parameters.StartUrl, UriKind.Absolute, out startUrl)) violations.Add($"StartUrl '{parameters.StartUrl}' is not an absolute URI");
        else if (!policy.TrustAllOrigins && !policy.IsTrustedOrigin(startUrl)) violations.Add($"StartUrl '{startUrl}' is not a trusted origin");

        InfiniFrameHostWindowState initialState = InfiniFrameHostWindowState.Normal;
        if (parameters.Minimized) initialState |= InfiniFrameHostWindowState.Minimized;
        if (parameters.Maximized) initialState |= InfiniFrameHostWindowState.Maximized;
        if (parameters.FullScreen) initialState |= InfiniFrameHostWindowState.FullScreen;
        if (initialState != InfiniFrameHostWindowState.Normal && !capabilities.HasFlag(InfiniFrameHostCapabilities.WindowState))
            violations.Add($"initial state {initialState} (needs {InfiniFrameHostCapabilities.WindowState})");

        ReadOnlyMemory<byte> icon = ReadOnlyMemory<byte>.Empty;
        if (!string.Equals(parameters.WindowIconFile, defaults.WindowIconFile, StringComparison.Ordinal) && parameters.WindowIconFile is {} iconPath) {
            if (!capabilities.HasFlag(InfiniFrameHostCapabilities.Icon)) violations.Add($"an icon (needs {InfiniFrameHostCapabilities.Icon})");
            else {
                try { icon = ReadPngIcon(iconPath); }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException) {
                    violations.Add($"icon '{iconPath}': {exception.Message}");
                }
            }
        }

        Size? initialSize = null;
        if (!parameters.UseOsDefaultSize) {
            if (parameters.Width <= 0 || parameters.Height <= 0) violations.Add($"size {parameters.Width}x{parameters.Height}");
            else initialSize = new Size(parameters.Width, parameters.Height);
        }
        Point? initialLocation = parameters.UseOsDefaultLocation ? null : new Point(parameters.Left, parameters.Top);

        var schemes = new SortedSet<string>(StringComparer.Ordinal);
        foreach (string scheme in events.CustomScheme.Snapshot.Keys) {
            if (!Uri.CheckSchemeName(scheme)) violations.Add($"custom scheme name '{scheme}'");
            else schemes.Add(scheme.ToLowerInvariant());
        }

        if (violations.Count != 0)
            throw new NotSupportedException(
                $"The host provider '{providerName}' cannot honour this window configuration: {string.Join("; ", violations)}. " +
                "Remove the setting or use a provider that declares the capability. No default-engine fallback is performed.");

        return new InfiniFrameHostWindowRequest {
            WindowId = windowId,
            StartUrl = startUrl!,
            CustomSchemes = [.. schemes],
            TrustedOrigins = [.. policy.TrustedOrigins],
            TrustAllOrigins = policy.TrustAllOrigins,
            BridgeScript = InfiniFrameHostBridgeScript.Value,
            BridgeContractVersion = InfiniFrameHostContract.BridgeContractVersion,
            Title = parameters.Title ?? string.Empty,
            InitialLocation = initialLocation,
            InitialSize = initialSize,
            InitialState = initialState,
            InitialIconPng = icon,
            BrowserPolicy = new InfiniFrameHostBrowserPolicy {
                WebSecurity = parameters.WebSecurityEnabled,
                DevTools = parameters.DevToolsEnabled,
                ContextMenu = parameters.ContextMenuEnabled,
                GrantBrowserPermissions = parameters.GrantBrowserPermissions,
                JavaScriptClipboardAccess = parameters.JavascriptClipboardAccessEnabled,
                MediaStream = parameters.MediaStreamEnabled,
                MediaAutoplay = parameters.MediaAutoplayEnabled,
                FileSystemAccess = parameters.FileSystemAccessEnabled,
                IgnoreCertificateErrors = parameters.IgnoreCertificateErrorsEnabled,
                UserAgent = string.IsNullOrEmpty(parameters.UserAgent) ? null : parameters.UserAgent
            }
        };
    }

    internal static byte[] ReadPngIcon(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        using FileStream stream = File.OpenRead(path);
        if (stream.Length is < 8 or > MaxIconBytes)
            throw new InvalidDataException($"Host icons must be PNG files of at most {MaxIconBytes} bytes.");
        byte[] bytes = new byte[checked((int)stream.Length)];
        stream.ReadExactly(bytes);
        if (!bytes.AsSpan(0, PngSignature.Length).SequenceEqual(PngSignature))
            throw new InvalidDataException("Host icons must use the PNG format.");
        return bytes;
    }

    private static void RejectChanged<T>(List<string> violations, string name, T actual, T expected) {
        if (!EqualityComparer<T>.Default.Equals(actual, expected)) violations.Add(name);
    }

    private static void RequireForHandlers(List<string> violations, InfiniFrameHostCapabilities declared,
        InfiniFrameHostCapabilities required, string eventName, int handlerCount) {
        if (handlerCount != 0 && !declared.HasFlag(required)) violations.Add($"{eventName} handlers (needs {required})");
    }
}
