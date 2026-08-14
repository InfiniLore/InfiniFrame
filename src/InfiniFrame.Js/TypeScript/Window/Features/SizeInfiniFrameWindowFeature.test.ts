import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("SizeInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
