import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("LifecycleInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
