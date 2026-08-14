import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("NotificationsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
