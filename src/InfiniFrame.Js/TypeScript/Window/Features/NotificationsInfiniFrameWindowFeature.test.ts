import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";
import type {NotificationsInfiniFrameWindowFeature} from "./NotificationsInfiniFrameWindowFeature";

describe("NotificationsInfiniFrameWindowFeature", () => {
    let feature: NotificationsInfiniFrameWindowFeature;
    let messaging: ReturnType<
        typeof import("./_testHelpers").createMessagingMock
    >;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./NotificationsInfiniFrameWindowFeature");
        feature = new mod.NotificationsInfiniFrameWindowFeature();
    });

    it("showMessageAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("OK"));
        await feature.showMessageAsync("Test", "Hello");
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("showNotification posts command", () => {
        feature.showNotification("Test", "Hello");
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});
