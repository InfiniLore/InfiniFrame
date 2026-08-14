import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("WebMessagingInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./WebMessagingInfiniFrameWindowFeature");
        feature = new mod.WebMessagingInfiniFrameWindowFeature();
    });

    it("sendWebMessage posts command", () => { feature.sendWebMessage("hello"); expect(messaging.sendMessageToHost).toHaveBeenCalled(); });
});
