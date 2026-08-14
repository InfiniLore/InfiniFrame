import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("PageNavigationInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

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
    it("getCurrentUrlAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("https://example.com"));
        await feature.getCurrentUrlAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getCurrentUriAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("app://localhost/page"));
        await feature.getCurrentUriAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
});
