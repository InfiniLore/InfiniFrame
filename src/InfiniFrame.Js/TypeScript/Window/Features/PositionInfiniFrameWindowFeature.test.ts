import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("PositionInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
