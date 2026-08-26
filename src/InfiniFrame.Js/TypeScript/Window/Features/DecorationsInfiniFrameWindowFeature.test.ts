import {beforeEach, describe, expect, it} from "vitest";
import {setupFeature} from "./_testHelpers";

describe("DecorationsInfiniFrameWindowFeature", () => {
    let feature: any;
    let messaging: ReturnType<typeof import("./_testHelpers").createMessagingMock>;

    beforeEach(async () => {
        messaging = setupFeature();
        const mod = await import("./DecorationsInfiniFrameWindowFeature");
        feature = new mod.DecorationsInfiniFrameWindowFeature();
    });

    it("isChromelessAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isChromelessAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("isTransparentAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.isTransparentAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("backgroundColorAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("#ffffff"));
        await feature.backgroundColorAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getTitleAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("Test Title"));
        await feature.getTitleAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getIconFilePathAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("/icon.png"));
        await feature.getIconFilePathAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("getLimitLinuxWindowTitleLengthAsync calls get", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(true));
        await feature.getLimitLinuxWindowTitleLengthAsync();
        expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
    });
    it("setTransparent posts command", () => {
        feature.setTransparent(true);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setBackgroundColor posts command", () => {
        feature.setBackgroundColor("#000000");
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setTitle posts command", () => {
        feature.setTitle("New Title");
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setIconFile posts command", () => {
        feature.setIconFile("/new-icon.png");
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
    it("setLimitLinuxWindowTitleLength posts command", () => {
        feature.setLimitLinuxWindowTitleLength(false);
        expect(messaging.sendMessageToHost).toHaveBeenCalled();
    });
});
