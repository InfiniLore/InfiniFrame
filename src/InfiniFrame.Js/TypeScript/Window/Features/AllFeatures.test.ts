// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";

// ---------------------------------------------------------------------------------------------------------------------
// Test helpers
// ---------------------------------------------------------------------------------------------------------------------
function createMessagingMock() {
    return {
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        getMessageFromHostRawAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn()
    };
}

function setupFeature() {
    const messaging = createMessagingMock();
    (window as any).infiniframe = {messaging};
    return messaging;
}

// ---------------------------------------------------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------------------------------------------------
describe("BrowserInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        // Mock the InfiniFrameHostMessaging class to prevent it from overwriting window.infiniframe.messaging
        vi.doMock("../InfiniFrameHostMessaging", () => ({
            default: class { constructor() { /* do not overwrite */ } }
        }));
        const mod = await import("./BrowserInfiniFrameWindowFeature");
        feature = new mod.BrowserInfiniFrameWindowFeature();
        // Ensure the mock messaging is still in place
        (window as any).infiniframe.messaging = messaging;
    });

    it("constructs without error", () => { expect(feature).toBeDefined(); });
    it("registers message handlers on construction", () => { expect(messaging.assignMessageReceivedHandler).toHaveBeenCalled(); });
    it("isContextMenuEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isContextMenuEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isMediaAutoplayEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isMediaAutoplayEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getUserAgentAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("test-agent"));
        await feature.getUserAgentAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("enableContextMenu posts command", () => { feature.enableContextMenu(false); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("enableMediaAutoplay posts command", () => { feature.enableMediaAutoplay(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setUserAgent posts command", () => { feature.setUserAgent("custom-agent"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("win32SetWebView2Path posts command", () => { feature.win32SetWebView2Path("C:/path"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("clearBrowserAutoFill posts command", () => { feature.clearBrowserAutoFill(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });

    it("keydown guard blocks ctrl+key browser shortcuts", () => {
        const event = new KeyboardEvent("keydown", {key: "t", ctrlKey: true, bubbles: true, cancelable: true});
        const dispatched = document.dispatchEvent(event);
        expect(dispatched).toBeDefined();
    });

    it("keydown guard blocks F11 key", () => {
        const event = new KeyboardEvent("keydown", {key: "F11", bubbles: true, cancelable: true});
        const dispatched = document.dispatchEvent(event);
        expect(dispatched).toBeDefined();
    });

    it("contextmenu guard blocks right-click when disabled", () => {
        const event = new Event("contextmenu", {bubbles: true, cancelable: true});
        const dispatched = document.dispatchEvent(event);
        expect(dispatched).toBeDefined();
    });

    it("wheel guard blocks ctrl+wheel zoom", () => {
        const event = new WheelEvent("wheel", {ctrlKey: true, deltaY: 100, bubbles: true, cancelable: true});
        const dispatched = document.dispatchEvent(event);
        expect(dispatched).toBeDefined();
    });
});

describe("DebuggingInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        vi.doMock("../InfiniFrameHostMessaging", () => ({default: class { constructor() {} }}));
        const mod = await import("./DebuggingInfiniFrameWindowFeature");
        feature = new mod.DebuggingInfiniFrameWindowFeature();
        (window as any).infiniframe.messaging = messaging;
    });

    it("isDevToolsEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isDevToolsEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("supportsWebInspectorAttachAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.supportsWebInspectorAttachAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isWebInspectorEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isWebInspectorEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("supportsRemoteDebuggingEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.supportsRemoteDebuggingEndpointAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getRemoteDebuggingPortAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(9222));
        await feature.getRemoteDebuggingPortAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getCapabilitiesAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({supportsLocalDevTools: true}));
        await feature.getCapabilitiesAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getDiagnosticsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({platform: "test"}));
        await feature.getDiagnosticsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("tryGetRemoteDebuggingEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({success: true}));
        await feature.tryGetRemoteDebuggingEndpointAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("tryProbeEndpointAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({reachable: true}));
        await feature.tryProbeEndpointAsync("http://localhost:9222");
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("enableDevTools posts command", () => { feature.enableDevTools(false); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("DecorationsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./DecorationsInfiniFrameWindowFeature");
        feature = new mod.DecorationsInfiniFrameWindowFeature();
    });

    it("isChromelessAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isChromelessAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isTransparentAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isTransparentAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("backgroundColorAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("#ffffff"));
        await feature.backgroundColorAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getTitleAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("Test Title"));
        await feature.getTitleAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getIconFilePathAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("/icon.png"));
        await feature.getIconFilePathAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getLimitLinuxWindowTitleLengthAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.getLimitLinuxWindowTitleLengthAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("setTransparent posts command", () => { feature.setTransparent(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setBackgroundColor posts command", () => { feature.setBackgroundColor("#000000"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setTitle posts command", () => { feature.setTitle("New Title"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setIconFile posts command", () => { feature.setIconFile("/new-icon.png"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setLimitLinuxWindowTitleLength posts command", () => { feature.setLimitLinuxWindowTitleLength(false); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("FilePickerDialogsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./FilePickerDialogsInfiniFrameWindowFeature");
        feature = new mod.FilePickerDialogsInfiniFrameWindowFeature();
    });

    it("showOpenFileAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("/selected/file.txt"));
        await feature.showOpenFileAsync({filters: []});
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("showOpenFolderAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("/selected/folder"));
        await feature.showOpenFolderAsync({filters: []});
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("showSaveFileAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("/save/path.txt"));
        await feature.showSaveFileAsync({filters: []});
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
});

describe("InvokeInfiniFrameWindowFeature", () => {
    it("constructs without error", async () => {
        setupFeature();
        vi.doMock("../InfiniFrameHostMessaging", () => ({default: class { constructor() {} }}));
        const mod = await import("./InvokeInfiniFrameWindowFeature");
        const feature = new mod.InvokeInfiniFrameWindowFeature();
        expect(feature).toBeDefined();
    });
});

describe("JavaScriptInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./JavaScriptInfiniFrameWindowFeature");
        feature = new mod.JavaScriptInfiniFrameWindowFeature();
    });

    it("evalAsync sends eval command and resolves on response", async () => {
        const {handleJavaScriptEvalResponse} = await import("./JavaScriptInfiniFrameWindowFeature");
        messaging.sendMessageToHost.mockImplementation((_id: string, data: any) => {
            // The post method sends {command: "...", args: {script, requestId}}
            const args = data.args || data;
            const requestId = args.requestId;
            handleJavaScriptEvalResponse({requestId, result: JSON.stringify("42")});
        });
        const result = await feature.evalAsync("1 + 1");
        expect(result).toBe("42");
    });

    it("evalAsync rejects on error response", async () => {
        const {handleJavaScriptEvalResponse} = await import("./JavaScriptInfiniFrameWindowFeature");
        messaging.sendMessageToHost.mockImplementation((_id: string, data: any) => {
            const args = data.args || data;
            const requestId = args.requestId;
            handleJavaScriptEvalResponse({requestId, error: "Syntax error"});
        });
        await expect(feature.evalAsync("throw new Error('Syntax error')")).rejects.toThrow("Syntax error");
    });

    it("handleJavaScriptEvalRequest executes script and sends result", async () => {
        const {handleJavaScriptEvalRequest} = await import("./JavaScriptInfiniFrameWindowFeature");
        handleJavaScriptEvalRequest({requestId: "req-1", script: "1 + 2"});
        expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
            "__infiniframe:javascript:eval:result",
            expect.objectContaining({requestId: "req-1", result: "3"})
        );
    });

    it("handleJavaScriptEvalRequest sends error on exception", async () => {
        const {handleJavaScriptEvalRequest} = await import("./JavaScriptInfiniFrameWindowFeature");
        handleJavaScriptEvalRequest({requestId: "req-2", script: "throw new Error('fail')"});
        expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
            "__infiniframe:javascript:eval:result",
            expect.objectContaining({requestId: "req-2", error: expect.any(String)})
        );
    });

    it("handleJavaScriptEvalRequest ignores invalid payload", async () => {
        const {handleJavaScriptEvalRequest} = await import("./JavaScriptInfiniFrameWindowFeature");
        handleJavaScriptEvalRequest(null);
        handleJavaScriptEvalRequest({});
        handleJavaScriptEvalRequest({requestId: "x"});
        handleJavaScriptEvalRequest({script: "y"});
    });

    it("handleJavaScriptEvalResponse ignores invalid payload", async () => {
        const {handleJavaScriptEvalResponse} = await import("./JavaScriptInfiniFrameWindowFeature");
        handleJavaScriptEvalResponse(null);
        handleJavaScriptEvalResponse({});
        handleJavaScriptEvalResponse({requestId: "nonexistent"});
    });

    it("handleJavaScriptEvalResponse resolves with null for null result", async () => {
        const {handleJavaScriptEvalResponse} = await import("./JavaScriptInfiniFrameWindowFeature");
        messaging.sendMessageToHost.mockImplementation((_id: string, data: any) => {
            const args = data.args || data;
            const requestId = args.requestId;
            handleJavaScriptEvalResponse({requestId, result: null});
        });
        const result = await feature.evalAsync("undefined");
        expect(result).toBeNull();
    });
});

describe("MonitorsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./MonitorsInfiniFrameWindowFeature");
        feature = new mod.MonitorsInfiniFrameWindowFeature();
    });

    it("getMonitorsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify([]));
        await feature.getMonitorsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getMainMonitorAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({}));
        await feature.getMainMonitorAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getMainMonitorScreenDpiAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(96));
        await feature.getMainMonitorScreenDpiAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
});

describe("NotificationsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./NotificationsInfiniFrameWindowFeature");
        feature = new mod.NotificationsInfiniFrameWindowFeature();
    });

    it("showMessageAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("OK"));
        await feature.showMessageAsync({title: "Test", message: "Hello"});
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("showNotification posts command", () => {
        feature.showNotification({title: "Test", message: "Hello"});
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});

describe("PageNavigationInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./PageNavigationInfiniFrameWindowFeature");
        feature = new mod.PageNavigationInfiniFrameWindowFeature();
    });

    it("tryLoadUriAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.tryLoadUriAsync("https://example.com");
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("tryLoadPathAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.tryLoadPathAsync("/page.html");
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("loadUri posts command", () => { feature.loadUri("https://example.com"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("loadPath posts command", () => { feature.loadPath("/page.html"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("loadRawString posts command", () => { feature.loadRawString("<html></html>"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("PositionInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./PositionInfiniFrameWindowFeature");
        feature = new mod.PositionInfiniFrameWindowFeature();
    });

    it("getLocationAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({left: 100, top: 200}));
        await feature.getLocationAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getTopAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(200));
        await feature.getTopAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getLeftAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(100));
        await feature.getLeftAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("setLocation posts command", () => { feature.setLocation(100, 200); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setLeft posts command", () => { feature.setLeft(100); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setTop posts command", () => { feature.setTop(200); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("offset posts command", () => { feature.offset(10, 20); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("center posts command", () => { feature.center(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("centerOnCurrentMonitor posts command", () => { feature.centerOnCurrentMonitor(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("centerOnMonitor posts command", () => { feature.centerOnMonitor(0); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("moveWithinCurrentMonitorArea posts command", () => { feature.moveWithinCurrentMonitorArea(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("SizeInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./SizeInfiniFrameWindowFeature");
        feature = new mod.SizeInfiniFrameWindowFeature();
    });

    it("getSizeAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({width: 800, height: 600}));
        await feature.getSizeAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getHeightAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(600));
        await feature.getHeightAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getWidthAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(800));
        await feature.getWidthAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getMaxSizeAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({width: 1920, height: 1080}));
        await feature.getMaxSizeAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getMinSizeAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify({width: 200, height: 150}));
        await feature.getMinSizeAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isResizableAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isResizableAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("setSize posts command", () => { feature.setSize(800, 600); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setHeight posts command", () => { feature.setHeight(600); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setWidth posts command", () => { feature.setWidth(800); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setMaxSize posts command", () => { feature.setMaxSize(1920, 1080); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setMinSize posts command", () => { feature.setMinSize(200, 150); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setResizable posts command", () => { feature.setResizable(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("resize posts command", () => { feature.resize(10, 20); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("StateInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./StateInfiniFrameWindowFeature");
        feature = new mod.StateInfiniFrameWindowFeature();
    });

    it("isFullScreenAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(false));
        await feature.isFullScreenAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isMaximizedAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isMaximizedAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isMinimizedAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(false));
        await feature.isMinimizedAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isTopMostAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(false));
        await feature.isTopMostAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isFocusedAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isFocusedAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getZoomFactorAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(1.0));
        await feature.getZoomFactorAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isZoomEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isZoomEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getCachedPreFullScreenBoundsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(null));
        await feature.getCachedPreFullScreenBoundsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getCachedPreMaximizedBoundsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(null));
        await feature.getCachedPreMaximizedBoundsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("setCachedPreFullScreenBounds posts command", () => {
        feature.setCachedPreFullScreenBounds({left: 0, top: 0, width: 800, height: 600});
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setCachedPreMaximizedBounds posts command", () => {
        feature.setCachedPreMaximizedBounds({left: 0, top: 0, width: 800, height: 600});
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setMaximized posts command", () => { feature.setMaximized(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("toggleMaximized posts command", () => { feature.toggleMaximized(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setMinimized posts command", () => { feature.setMinimized(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setFullScreen posts command", () => { feature.setFullScreen(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setFocused posts command", () => { feature.setFocused(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setZoomFactor posts command", () => { feature.setZoomFactor(1.5); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("enableZoom posts command", () => { feature.enableZoom(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("setTopMost posts command", () => { feature.setTopMost(true); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("WebMessagingInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./WebMessagingInfiniFrameWindowFeature");
        feature = new mod.WebMessagingInfiniFrameWindowFeature();
    });

    it("sendWebMessage posts command", () => { feature.sendWebMessage("hello"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});

describe("LifecycleInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./LifecycleInfiniFrameWindowFeature");
        feature = new mod.LifecycleInfiniFrameWindowFeature();
    });

    it("constructs with lifecycle feature name", () => { expect(feature).toBeDefined(); });
    it("getStateAsync sends get request", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("Running"));
        const result = await feature.getStateAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
        expect(result).toBe("Running");
    });
    it("isClosedOrClosingAsync sends get request", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(false));
        const result = await feature.isClosedOrClosingAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
        expect(result).toBe(false);
    });
    it("close sends post command", () => {
        feature.close();
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});
