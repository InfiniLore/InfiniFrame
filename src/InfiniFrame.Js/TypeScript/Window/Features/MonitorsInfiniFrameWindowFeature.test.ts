import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("MonitorsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
