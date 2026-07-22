// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import type {InfiniFrameHostMessaging} from "../Contracts";
import {SendToHostMessageIds} from "../Contracts";
import {InfiniFrameWindow} from "./InfiniFrameWindow";

// ---------------------------------------------------------------------------------------------------------------------
// Test components
// ---------------------------------------------------------------------------------------------------------------------
type FeatureName = keyof InfiniFrameWindow["features"];
type FeatureMethod = { method: string; command: string; parameters?: unknown[]; args?: unknown; result?: unknown };
type FeatureContract = { feature: FeatureName; gets?: FeatureMethod[]; posts?: FeatureMethod[] };

const contracts: FeatureContract[] = [
    {
        feature: "browser",
        gets: [
            ["isContextMenuEnabledAsync", "isContextMenuEnabled", true],
            ["isMediaAutoplayEnabledAsync", "isMediaAutoplayEnabled", true],
            ["getUserAgentAsync", "userAgent", "test-agent"],
            ["isFileSystemAccessEnabledAsync", "isFileSystemAccessEnabled", true],
            ["isWebSecurityEnabledAsync", "isWebSecurityEnabled", true],
            ["isJavascriptClipboardAccessEnabledAsync", "isJavascriptClipboardAccessEnabled", true],
            ["isMediaStreamEnabledAsync", "isMediaStreamEnabled", true],
            ["isIgnoreCertificateErrorsEnabledAsync", "isIgnoreCertificateErrorsEnabled", true],
            ["getGrantBrowserPermissionsAsync", "grantBrowserPermissions", true],
            ["isSmoothScrollingEnabledAsync", "isSmoothScrollingEnabled", true],
            ["getBrowserControlInitParametersAsync", "browserControlInitParameters", "--test"]
        ].map(([method, command, result]) => ({method: method as string, command: command as string, result})),
        posts: [
            {method: "enableContextMenu", command: "enableContextMenu", parameters: [false], args: {enabled: false}},
            {method: "enableMediaAutoplay", command: "enableMediaAutoplay", parameters: [false], args: {enabled: false}},
            {method: "setUserAgent", command: "setUserAgent", parameters: [null], args: {userAgent: null}},
            {method: "win32SetWebView2Path", command: "win32SetWebView2Path", parameters: ["C:/WebView2"], args: {path: "C:/WebView2"}},
            {method: "clearBrowserAutoFill", command: "clearBrowserAutoFill"}
        ]
    },
    {
        feature: "debugging",
        gets: [
            ["isDevToolsEnabledAsync", "isDevToolsEnabled", true],
            ["supportsWebInspectorAttachAsync", "supportsWebInspectorAttach", true],
            ["isWebInspectorEnabledAsync", "isWebInspectorEnabled", true],
            ["supportsRemoteDebuggingEndpointAsync", "supportsRemoteDebuggingEndpoint", true],
            ["getRemoteDebuggingPortAsync", "remoteDebuggingPort", 9222],
            ["getCapabilitiesAsync", "capabilities", {supportsLocalDevTools: true}],
            ["getDiagnosticsAsync", "diagnostics", {platform: "test"}],
            ["tryGetRemoteDebuggingEndpointAsync", "remoteDebuggingEndpoint", {success: true, endpoint: "http://localhost:9222", reason: null}],
            ["tryProbeEndpointAsync", "probeEndpoint", {success: false, endpoint: null, reason: "test"}]
        ].map(([method, command, result]) => ({method: method as string, command: command as string, result})),
        posts: [{method: "enableDevTools", command: "enableDevTools", parameters: [false], args: {enabled: false}}]
    },
    {
        feature: "decorations",
        gets: [
            ["isChromelessAsync", "isChromeless", false],
            ["isTransparentAsync", "isTransparent", false],
            ["getTitleAsync", "title", "Native title"],
            ["getIconFilePathAsync", "iconFilePath", "icon.ico"],
            ["getLimitLinuxWindowTitleLengthAsync", "limitLinuxWindowTitleLength", true]
        ].map(([method, command, result]) => ({method: method as string, command: command as string, result})),
        posts: [
            {method: "setTransparent", command: "setTransparent", parameters: [false], args: {enabled: false}},
            {method: "setTitle", command: "setTitle", parameters: [null], args: {title: null}},
            {method: "setIconFile", command: "setIconFile", parameters: ["icon.ico"], args: {iconFilePath: "icon.ico"}},
            {method: "setLimitLinuxWindowTitleLength", command: "setLimitLinuxWindowTitleLength", parameters: [false], args: {enabled: false}}
        ]
    },
    {
        feature: "filePickerDialogs",
        gets: [
            {method: "showOpenFileAsync", command: "showOpenFile", parameters: ["Open", "/tmp", true, [{name: "Text", extensions: ["txt"]}]], args: {title: "Open", defaultPath: "/tmp", multiSelect: true, filters: [{name: "Text", extensions: ["txt"]}]}, result: ["/tmp/a.txt"]},
            {method: "showOpenFolderAsync", command: "showOpenFolder", parameters: ["Folder", "/tmp", true], args: {title: "Folder", defaultPath: "/tmp", multiSelect: true}, result: ["/tmp"]},
            {method: "showSaveFileAsync", command: "showSaveFile", parameters: ["Save", "/tmp/a.txt", null], args: {title: "Save", defaultPath: "/tmp/a.txt", filters: null}, result: "/tmp/a.txt"}
        ]
    },
    {
        feature: "lifecycle",
        gets: [
            {method: "getStateAsync", command: "state", result: "running"},
            {method: "isClosedOrClosingAsync", command: "isClosedOrClosing", result: false}
        ],
        posts: [{method: "close", command: "close"}]
    },
    {
        feature: "monitors",
        gets: [
            {method: "getMonitorsAsync", command: "monitors", result: []},
            {method: "getMainMonitorAsync", command: "mainMonitor", result: {monitorArea: {x: 0, y: 0, width: 1920, height: 1080}, workArea: {x: 0, y: 0, width: 1920, height: 1040}, scale: 1}},
            {method: "getMainMonitorScreenDpiAsync", command: "mainMonitorScreenDpi", result: 96}
        ]
    },
    {
        feature: "notifications",
        gets: [{method: "showMessageAsync", command: "showMessage", parameters: ["Title", "Text", "yesNo", "question"], args: {title: "Title", text: "Text", buttons: "yesNo", icon: "question"}, result: "yes"}],
        posts: [{method: "showNotification", command: "showNotification", parameters: ["Title", "Body"], args: {title: "Title", body: "Body"}}]
    },
    {
        feature: "pageNavigation",
        gets: [
            {method: "tryLoadUriAsync", command: "tryLoadUri", parameters: ["https://example.test"], args: {uri: "https://example.test"}, result: true},
            {method: "tryLoadPathAsync", command: "tryLoadPath", parameters: ["index.html"], args: {path: "index.html"}, result: true}
        ],
        posts: [
            {method: "loadUri", command: "loadUri", parameters: ["https://example.test"], args: {uri: "https://example.test"}},
            {method: "loadPath", command: "loadPath", parameters: ["index.html"], args: {path: "index.html"}},
            {method: "loadRawString", command: "loadRawString", parameters: ["<p>test</p>"], args: {content: "<p>test</p>"}}
        ]
    },
    {
        feature: "position",
        gets: [
            {method: "getLocationAsync", command: "location", result: {x: 10, y: 20}},
            {method: "getTopAsync", command: "top", result: 20},
            {method: "getLeftAsync", command: "left", result: 10}
        ],
        posts: [
            {method: "setLocation", command: "setLocation", parameters: [10, 20], args: {left: 10, top: 20}},
            {method: "setLeft", command: "setLeft", parameters: [10], args: {left: 10}},
            {method: "setTop", command: "setTop", parameters: [20], args: {top: 20}},
            {method: "offset", command: "offset", parameters: [1, 2], args: {left: 1, top: 2}},
            {method: "center", command: "center"},
            {method: "centerOnCurrentMonitor", command: "centerOnCurrentMonitor"},
            {method: "centerOnMonitor", command: "centerOnMonitor", parameters: [1], args: {monitorIndex: 1}},
            {method: "moveWithinCurrentMonitorArea", command: "moveWithinCurrentMonitorArea", parameters: [10, 20], args: {left: 10, top: 20}}
        ]
    },
    {
        feature: "size",
        gets: [
            ["getSizeAsync", "size", {width: 800, height: 600}], ["getHeightAsync", "height", 600], ["getWidthAsync", "width", 800],
            ["getMaxSizeAsync", "maxSize", {width: 1600, height: 1200}], ["getMaxHeightAsync", "maxHeight", 1200], ["getMaxWidthAsync", "maxWidth", 1600],
            ["getMinSizeAsync", "minSize", {width: 320, height: 200}], ["getMinHeightAsync", "minHeight", 200], ["getMinWidthAsync", "minWidth", 320],
            ["isResizableAsync", "isResizable", true]
        ].map(([method, command, result]) => ({method: method as string, command: command as string, result})),
        posts: [
            ["setSize", "setSize", [800, 600], {width: 800, height: 600}], ["setHeight", "setHeight", [600], {height: 600}], ["setWidth", "setWidth", [800], {width: 800}],
            ["setMaxSize", "setMaxSize", [1600, 1200], {width: 1600, height: 1200}], ["setMaxHeight", "setMaxHeight", [1200], {height: 1200}], ["setMaxWidth", "setMaxWidth", [1600], {width: 1600}],
            ["setMinSize", "setMinSize", [320, 200], {width: 320, height: 200}], ["setMinHeight", "setMinHeight", [200], {height: 200}], ["setMinWidth", "setMinWidth", [320], {width: 320}],
            ["resize", "resize", [10, 20, "bottomRight"], {widthOffset: 10, heightOffset: 20, origin: "bottomRight"}], ["setResizable", "setResizable", [false], {resizable: false}]
        ].map(([method, command, parameters, args]) => ({method: method as string, command: command as string, parameters: parameters as unknown[], args}))
    },
    {
        feature: "state",
        gets: [
            ["isFullScreenAsync", "isFullScreen", false], ["isMaximizedAsync", "isMaximized", false], ["isMinimizedAsync", "isMinimized", false],
            ["isTopMostAsync", "isTopMost", false], ["isFocusedAsync", "isFocused", true], ["getZoomFactorAsync", "zoomFactor", 100],
            ["isZoomEnabledAsync", "isZoomEnabled", true], ["getCachedPreFullScreenBoundsAsync", "cachedPreFullScreenBounds", {x: 0, y: 0, width: 800, height: 600}],
            ["getCachedPreMaximizedBoundsAsync", "cachedPreMaximizedBounds", {x: 0, y: 0, width: 800, height: 600}]
        ].map(([method, command, result]) => ({method: method as string, command: command as string, result})),
        posts: [
            ["setMaximized", "setMaximized", [false], {maximized: false}], ["toggleMaximized", "toggleMaximized", [], undefined], ["setMinimized", "setMinimized", [false], {minimized: false}],
            ["setFullScreen", "setFullScreen", [false], {fullScreen: false}], ["setFocused", "setFocused", [], undefined], ["setZoomFactor", "setZoomFactor", [125], {zoom: 125}],
            ["enableZoom", "enableZoom", [false], {enabled: false}], ["setTopMost", "setTopMost", [false], {topMost: false}]
        ].map(([method, command, parameters, args]) => ({method: method as string, command: command as string, parameters: parameters as unknown[], args}))
    },
    {
        feature: "webMessaging",
        posts: [{method: "sendWebMessage", command: "sendWebMessage", parameters: ["hello"], args: {message: "hello"}}]
    }
];

describe.each(contracts)("$feature window feature", ({feature, gets = [], posts = []}) => {
    const sendMessageToHost = vi.fn();
    const getMessageFromHostAsync = vi.fn();
    let windowApi: InfiniFrameWindow;

    beforeEach(() => {
        vi.clearAllMocks();
        const messaging = {
            sendMessageToHost,
            getMessageFromHostAsync,
            assignMessageReceivedHandler: vi.fn(),
            unregisterMessageReceivedHandler: vi.fn()
        } as unknown as InfiniFrameHostMessaging;
        windowApi = new InfiniFrameWindow();
        window.infiniframe = {messaging, window: windowApi, utils: {setPointerCapture: vi.fn(), releasePointerCapture: vi.fn()}};
    });

    it.each(gets)("$method maps to $command", async ({method, command, parameters = [], args, result}) => {
        getMessageFromHostAsync.mockResolvedValueOnce(JSON.stringify(result));

        const actual = await (windowApi.features[feature] as any)[method](...parameters);

        expect(getMessageFromHostAsync).toHaveBeenCalledWith(`__infiniframe:window:features:${feature}:${command}`, args);
        expect(actual).toEqual(result);
    });

    it.each(posts)("$method maps to $command", ({method, command, parameters = [], args}) => {
        (windowApi.features[feature] as any)[method](...parameters);

        expect(sendMessageToHost).toHaveBeenCalledWith(SendToHostMessageIds.windowFeatureRequest, {
            command: `__infiniframe:window:features:${feature}:${command}`,
            args
        });
    });
});
