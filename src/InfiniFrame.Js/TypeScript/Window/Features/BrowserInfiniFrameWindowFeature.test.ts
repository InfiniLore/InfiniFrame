import {beforeEach, describe, expect, it, vi} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("BrowserInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

    beforeEach(async () => {
        vi.resetModules();
        messaging = setupFeature();
        vi.doMock("../InfiniFrameHostMessaging", () => ({default: class { constructor() {} }}));
        const mod = await import("./BrowserInfiniFrameWindowFeature");
        feature = new mod.BrowserInfiniFrameWindowFeature();
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
        document.dispatchEvent(event);
    });
    it("keydown guard blocks F11 key", () => {
        const event = new KeyboardEvent("keydown", {key: "F11", bubbles: true, cancelable: true});
        document.dispatchEvent(event);
    });
    it("contextmenu guard blocks right-click when disabled", () => {
        const event = new Event("contextmenu", {bubbles: true, cancelable: true});
        document.dispatchEvent(event);
    });
    it("wheel guard blocks ctrl+wheel zoom", () => {
        const event = new WheelEvent("wheel", {ctrlKey: true, deltaY: 100, bubbles: true, cancelable: true});
        document.dispatchEvent(event);
    });
    it("isFileSystemAccessEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isFileSystemAccessEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isWebSecurityEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isWebSecurityEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isJavascriptClipboardAccessEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isJavascriptClipboardAccessEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isMediaStreamEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isMediaStreamEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isIgnoreCertificateErrorsEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isIgnoreCertificateErrorsEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getGrantBrowserPermissionsAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.getGrantBrowserPermissionsAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isSmoothScrollingEnabledAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isSmoothScrollingEnabledAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getBrowserControlInitParametersAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("--flag"));
        await feature.getBrowserControlInitParametersAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("enableContextMenu posts with default true", () => { feature.enableContextMenu(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("enableMediaAutoplay posts with default true", () => { feature.enableMediaAutoplay(); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
    it("message handler updates contextMenuEnabled on valid payload", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setContextMenuEnabled")
        )?.[1];
        expect(handler).toBeDefined();
        handler!(JSON.stringify({enabled: false}));
    });
    it("message handler ignores null payload for contextMenu", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setContextMenuEnabled")
        )?.[1];
        handler!(null);
    });
    it("message handler ignores malformed JSON for contextMenu", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setContextMenuEnabled")
        )?.[1];
        handler!("not-json");
    });
    it("message handler updates zoomEnabled on valid payload", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setZoomEnabled")
        )?.[1];
        handler!(JSON.stringify({enabled: false}));
    });
    it("message handler ignores null payload for zoom", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setZoomEnabled")
        )?.[1];
        handler!(null);
    });
    it("message handler ignores malformed JSON for zoom", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setZoomEnabled")
        )?.[1];
        handler!("not-json");
    });
    it("message handler updates browserShortcutsEnabled on valid payload", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setBrowserShortcutsEnabled")
        )?.[1];
        handler!(JSON.stringify({enabled: false}));
    });
    it("message handler ignores null payload for browserShortcuts", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setBrowserShortcutsEnabled")
        )?.[1];
        handler!(null);
    });
    it("message handler ignores malformed JSON for browserShortcuts", () => {
        const handler = messaging.assignMessageReceivedHandler.mock.calls.find(
            (call: any[]) => typeof call[0] === "string" && call[0].includes("setBrowserShortcutsEnabled")
        )?.[1];
        handler!("not-json");
    });
    it("keydown guard blocks ctrl+shift+i", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "i", ctrlKey: true, shiftKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+n", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "n", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+w", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "w", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+r", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "r", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+p", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "p", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+u", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "u", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+j", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "j", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+l", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "l", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+o", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "o", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard blocks ctrl+h", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "h", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("keydown guard allows non-shortcut keys", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "a", bubbles: true, cancelable: true}));
    });
    it("zoom guard blocks ctrl+plus", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "+", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("zoom guard blocks ctrl+minus", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "-", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("zoom guard blocks ctrl+equal", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "=", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("zoom guard blocks ctrl+0", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "0", ctrlKey: true, bubbles: true, cancelable: true}));
    });
    it("zoom guard blocks F5", () => {
        document.dispatchEvent(new KeyboardEvent("keydown", {key: "F5", bubbles: true, cancelable: true}));
    });
});
