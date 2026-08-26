import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("StateInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
    it("setMaximized posts command", () => {
        feature.setMaximized(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("toggleMaximized posts command", () => {
        feature.toggleMaximized();
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setMinimized posts command", () => {
        feature.setMinimized(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setFullScreen posts command", () => {
        feature.setFullScreen(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setFocused posts command", () => {
        feature.setFocused(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setZoomFactor posts command", () => {
        feature.setZoomFactor(1.5);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("enableZoom posts command", () => {
        feature.enableZoom(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setTopMost posts command", () => {
        feature.setTopMost(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});
