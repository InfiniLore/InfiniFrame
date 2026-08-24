// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowFeatureDispatcherCommandTests {
    private static readonly CommandCase[] GetCommands = [
        Get("browser", "isContextMenuEnabled", "get_IsContextMenuEnabled"), Get("browser", "isMediaAutoplayEnabled", "get_IsMediaAutoplayEnabled"),
        Get("browser", "userAgent", "get_UserAgent"), Get("browser", "isFileSystemAccessEnabled", "get_IsFileSystemAccessEnabled"),
        Get("browser", "isWebSecurityEnabled", "get_IsWebSecurityEnabled"), Get("browser", "isJavascriptClipboardAccessEnabled", "get_IsJavascriptClipboardAccessEnabled"),
        Get("browser", "isMediaStreamEnabled", "get_IsMediaStreamEnabled"), Get("browser", "isIgnoreCertificateErrorsEnabled", "get_IsIgnoreCertificateErrorsEnabled"),
        Get("browser", "grantBrowserPermissions", "get_GrantBrowserPermissions"), Get("browser", "isSmoothScrollingEnabled", "get_IsSmoothScrollingEnabled"),
        Get("browser", "browserControlInitParameters", "get_BrowserControlInitParameters"),
        Get("debugging", "isDevToolsEnabled", "get_IsDevToolsEnabled"), Get("debugging", "supportsWebInspectorAttach", "get_SupportsWebInspectorAttach"),
        Get("debugging", "isWebInspectorEnabled", "get_IsWebInspectorEnabled"), Get("debugging", "supportsRemoteDebuggingEndpoint", "get_SupportsRemoteDebuggingEndpoint"),
        Get("debugging", "remoteDebuggingPort", "get_RemoteDebuggingPort"), Get("debugging", "capabilities", "get_Capabilities"),
        Get("debugging", "diagnostics", "GetDiagnostics"), Get("debugging", "remoteDebuggingEndpoint", "TryGetRemoteDebuggingEndpoint"),
        Get("debugging", "probeEndpoint", "TryProbeEndpoint"),
        Get("decorations", "isChromeless", "get_IsChromeless"), Get("decorations", "isTransparent", "get_IsTransparent"),
        Get("decorations", "backgroundColor", "get_BackgroundColor"),
        Get("decorations", "title", "get_Title"), Get("decorations", "iconFilePath", "get_IconFilePath"),
        Get("decorations", "limitLinuxWindowTitleLength", "get_LimitLinuxWindowTitleLength"),
        Get("filePickerDialogs", "showOpenFile", "ShowOpenFile", "{}"), Get("filePickerDialogs", "showOpenFolder", "ShowOpenFolder", "{}"),
        Get("filePickerDialogs", "showSaveFile", "ShowSaveFile", "{\"defaultFileName\":null}"),
        Get("filePickerDialogs", "showSaveFile", "ShowSaveFile", "{\"defaultFileName\":\"document.txt\"}"),
        Get("lifecycle", "state", "get_State"), Get("lifecycle", "isClosedOrClosing", "IsClosedOrClosing"),
        Get("monitors", "monitors", "GetMonitors"), Get("monitors", "mainMonitor", "GetMainMonitor"),
        Get("monitors", "mainMonitorScreenDpi", "GetMainMonitorScreenDpi"),
        Get("notifications", "showMessage", "ShowMessage", "{\"title\":\"Title\"}"),
        Get("pageNavigation", "tryLoadUri", "TryLoadUri", "{\"uri\":\"https://example.test\"}"),
        Get("pageNavigation", "tryLoadPath", "TryLoadPath", "{\"path\":\"index.html\"}"),
        Get("pageNavigation", "getCurrentUrl", "GetCurrentUrl"),
        Get("pageNavigation", "getCurrentUri", "GetCurrentUri"),
        Get("position", "location", "get_Location"), Get("position", "top", "get_Top"), Get("position", "left", "get_Left"),
        Get("size", "size", "get_Size"), Get("size", "height", "get_Height"), Get("size", "width", "get_Width"),
        Get("size", "maxSize", "get_MaxSize"), Get("size", "maxHeight", "get_MaxHeight"), Get("size", "maxWidth", "get_MaxWidth"),
        Get("size", "minSize", "get_MinSize"), Get("size", "minHeight", "get_MinHeight"), Get("size", "minWidth", "get_MinWidth"),
        Get("size", "isResizable", "get_IsResizable"),
        Get("state", "isFullScreen", "get_IsFullScreen"), Get("state", "isMaximized", "get_IsMaximized"),
        Get("state", "isMinimized", "get_IsMinimized"), Get("state", "isTopMost", "get_IsTopMost"), Get("state", "isFocused", "get_IsFocused"),
        Get("state", "zoomFactor", "get_ZoomFactor"), Get("state", "isZoomEnabled", "get_IsZoomEnabled"),
        Get("state", "cachedPreFullScreenBounds", "get_CachedPreFullScreenBounds"), Get("state", "cachedPreMaximizedBounds", "get_CachedPreMaximizedBounds")
    ];

    private static readonly CommandCase[] PostCommands = [
        Post("browser", "enableContextMenu", "EnableContextMenu", "{}"), Post("browser", "enableMediaAutoplay", "EnableMediaAutoplay", "{}"),
        Post("browser", "setUserAgent", "SetUserAgent", "{\"userAgent\":null}"), Post("browser", "win32SetWebView2Path", "Win32SetWebView2Path", "{\"path\":\"C:/WebView2\"}"),
        Post("browser", "clearBrowserAutoFill", "ClearBrowserAutoFill"), Post("debugging", "enableDevTools", "EnableDevTools", "{\"enabled\":true}"),
        Post("decorations", "setTransparent", "SetTransparent", "{}"), Post("decorations", "setBackgroundColor", "SetBackgroundColor", "{\"color\":\"#FF0000\"}"), Post("decorations", "setTitle", "SetTitle", "{\"title\":null}"),
        Post("decorations", "setIconFile", "SetIconFile", "{\"iconFilePath\":\"icon.ico\"}"),
        Post("decorations", "setLimitLinuxWindowTitleLength", "SetLimitLinuxWindowTitleLength", "{}"),
        Post("lifecycle", "close", "Close"), Post("notifications", "showNotification", "ShowNotification", "{\"title\":\"Title\",\"body\":\"Body\"}"),
        Post("pageNavigation", "loadUri", "Load", "{\"uri\":\"https://example.test\"}"), Post("pageNavigation", "loadPath", "Load", "{\"path\":\"index.html\"}"),
        Post("pageNavigation", "loadRawString", "LoadRawString", "{\"content\":\"<p>test</p>\"}"),
        Post("position", "setLocation", "SetLocation", "{\"left\":10,\"top\":20}"), Post("position", "setLeft", "SetLeft", "{\"left\":10}"),
        Post("position", "setTop", "SetTop", "{\"top\":20}"), Post("position", "offset", "Offset", "{\"left\":1.5,\"top\":2.5}"),
        Post("position", "center", "Center"), Post("position", "centerOnCurrentMonitor", "CenterOnCurrentMonitor"),
        Post("position", "centerOnMonitor", "CenterOnMonitor", "{\"monitorIndex\":1}"),
        Post("position", "moveWithinCurrentMonitorArea", "MoveWithinCurrentMonitorArea", "{\"left\":10.5,\"top\":20.5}"),
        Post("size", "setSize", "SetSize", "{\"width\":800,\"height\":600}"), Post("size", "setHeight", "SetHeight", "{\"height\":600}"),
        Post("size", "setMaxSize", "SetMaxSize", "{\"width\":1600,\"height\":1200}"), Post("size", "setMaxHeight", "SetMaxHeight", "{\"height\":1200}"),
        Post("size", "setMaxWidth", "SetMaxWidth", "{\"width\":1600}"), Post("size", "setMinSize", "SetMinSize", "{\"width\":320,\"height\":200}"),
        Post("size", "setMinHeight", "SetMinHeight", "{\"height\":200}"), Post("size", "setMinWidth", "SetMinWidth", "{\"width\":320}"),
        Post("size", "setWidth", "SetWidth", "{\"width\":800}"), Post("size", "resize", "Resize", "{\"widthOffset\":10,\"heightOffset\":20,\"origin\":\"bottomRight\"}"),
        Post("size", "setResizable", "SetResizable", "{}"),
        Post("state", "setCachedPreFullScreenBounds", "set_CachedPreFullScreenBounds", "{\"bounds\":{\"x\":1,\"y\":2,\"width\":800,\"height\":600}}"),
        Post("state", "setCachedPreMaximizedBounds", "set_CachedPreMaximizedBounds", "{\"bounds\":{\"x\":3,\"y\":4,\"width\":1024,\"height\":768}}"),
        Post("state", "setMaximized", "SetMaximized", "{}"), Post("state", "toggleMaximized", "ToggleMaximized"),
        Post("state", "setMinimized", "SetMinimized", "{}"), Post("state", "setFullScreen", "SetFullScreen", "{}"),
        Post("state", "setFocused", "SetFocused"), Post("state", "setZoomFactor", "SetZoomFactor", "{\"zoom\":125}"),
        Post("state", "enableZoom", "EnableZoom", "{}"), Post("state", "setTopMost", "SetTopMost", "{}"),
        Post("webMessaging", "sendWebMessage", "SendWebMessage", "{\"message\":\"hello\"}")
    ];

    [Test]
    public async Task EveryGetCommand_InvokesTheManifestMethodAndReturnsJson() {
        foreach (CommandCase command in GetCommands) {
            (IInfiniFrameWindow window, object featureObj) = CreateWindow(command.Feature);

            string response = WindowFeatureWebMessageRouter.Get(window, command.Feature, command.Command, Parse(command.Args));

            using JsonDocument _ = JsonDocument.Parse(response);
            bool platformShortCircuit = command.Feature == "debugging"
                && command.ManagedMember.StartsWith("Try", StringComparison.Ordinal)
                && !OperatingSystem.IsWindows()
                && !OperatingSystem.IsLinux();
            await Assert.That(WasMethodCalled(featureObj, command.ManagedMember))
                .IsEqualTo(!platformShortCircuit);
        }
    }

    [Test]
    public async Task EveryPostCommand_InvokesTheManifestMethod() {
        foreach (CommandCase command in PostCommands) {
            (IInfiniFrameWindow window, object featureObj) = CreateWindow(command.Feature);

            WindowFeatureWebMessageRouter.Post(window, command.Feature, command.Command, Parse(command.Args));

            await Assert.That(WasMethodCalled(featureObj, command.ManagedMember)).IsTrue();
        }
    }

    private static bool WasMethodCalled(object mockObj, string methodName) {
        if (mockObj is Mock<IBrowserInfiniFrameWindowFeature> m1) return Mock.Invocations(m1).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IDebuggingInfiniFrameWindowFeature> m2) return Mock.Invocations(m2).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IDecorationsInfiniFrameWindowFeature> m3) return Mock.Invocations(m3).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IFilePickerDialogsInfiniFrameWindowFeature> m4) return Mock.Invocations(m4).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<ILifecycleInfiniFrameWindowFeature> m5) return Mock.Invocations(m5).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IMonitorsInfiniFrameWindowFeature> m6) return Mock.Invocations(m6).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<INotificationsInfiniFrameWindowFeature> m7) return Mock.Invocations(m7).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IPageNavigationInfiniFrameWindowFeature> m8) return Mock.Invocations(m8).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IPositionInfiniFrameWindowFeature> m9) return Mock.Invocations(m9).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<ISizeInfiniFrameWindowFeature> m10) return Mock.Invocations(m10).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IStateInfiniFrameWindowFeature> m11) return Mock.Invocations(m11).Any(c => c.MemberName == methodName);
        if (mockObj is Mock<IWebMessagingInfiniFrameWindowFeature> m12) return Mock.Invocations(m12).Any(c => c.MemberName == methodName);

        return false;
    }

    private static (IInfiniFrameWindow Window, object Feature) CreateWindow(string featureName) {
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        window.Features.Returns(features.Object);
        object feature = featureName switch {
            "browser" => Assign(MockFactory.CreateBrowserMock(), assign: value => features.Browser.Returns(value)),
            "debugging" => Assign(MockFactory.CreateDebuggingMock(), assign: value => features.Debugging.Returns(value)),
            "decorations" => Assign(MockFactory.CreateDecorationsMock(), assign: value => features.Decorations.Returns(value)),
            "filePickerDialogs" => Assign(MockFactory.CreateFilePickerDialogsMock(), assign: value => features.FilePickerDialogs.Returns(value)),
            "lifecycle" => Assign(MockFactory.CreateLifecycleMock(), assign: value => features.Lifecycle.Returns(value)),
            "monitors" => Assign(MockFactory.CreateMonitorsMock(), assign: value => features.Monitors.Returns(value)),
            "notifications" => Assign(MockFactory.CreateNotificationsMock(), assign: value => features.Notifications.Returns(value)),
            "pageNavigation" => Assign(MockFactory.CreatePageNavigationMock(), assign: value => features.PageNavigation.Returns(value)),
            "position" => Assign(MockFactory.CreatePositionMock(), assign: value => features.Position.Returns(value)),
            "size" => Assign(MockFactory.CreateSizeMock(), assign: value => features.Size.Returns(value)),
            "state" => Assign(MockFactory.CreateStateMock(), assign: value => features.State.Returns(value)),
            "webMessaging" => Assign(MockFactory.CreateWebMessagingMock(), assign: value => features.WebMessaging.Returns(value)),
            _ => throw new ArgumentOutOfRangeException(nameof(featureName), featureName, null)
        };
        return (window.Object, feature);
    }

    private static Mock<T> Assign<T>(Mock<T> mock, Action<T> assign) where T : class {
        assign(mock.Object);
        return mock;
    }

    private static JsonElement? Parse(string? json) {
        if (json is null) return null;

        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private static CommandCase Get(string feature, string command, string managedMember, string? args = null)
        => new(feature, command, managedMember, args);
    private static CommandCase Post(string feature, string command, string managedMember, string? args = null)
        => new(feature, command, managedMember, args);

    private sealed record CommandCase(string Feature, string Command, string ManagedMember, string? Args);
}
