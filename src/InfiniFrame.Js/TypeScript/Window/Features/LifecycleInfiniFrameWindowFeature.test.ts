// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {beforeEach, describe, expect, it, vi} from "vitest";
import {LifecycleInfiniFrameWindowFeature} from "./LifecycleInfiniFrameWindowFeature";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
describe("LifecycleInfiniFrameWindowFeature", () => {
    let feature: LifecycleInfiniFrameWindowFeature;
    let messaging: ReturnType<typeof createMessagingMock>;

    beforeEach(() => {
        messaging = createMessagingMock();
        (window as any).infiniframe = {messaging};
        feature = new LifecycleInfiniFrameWindowFeature();
    });

    it("constructs with lifecycle feature name", () => {
        expect(feature).toBeDefined();
    });

    it("getStateAsync sends get request and returns result", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify("Running"));

        const result = await feature.getStateAsync();

        expect(messaging.getMessageFromHostAsync).toHaveBeenCalledWith(
            expect.stringContaining("lifecycle:state"),
            undefined
        );
        expect(result).toBe("Running");
    });

    it("isClosedOrClosingAsync sends get request", async () => {
        messaging.getMessageFromHostAsync.mockResolvedValue(JSON.stringify(false));

        const result = await feature.isClosedOrClosingAsync();

        expect(messaging.getMessageFromHostAsync).toHaveBeenCalledWith(
            expect.stringContaining("lifecycle:isClosedOrClosing"),
            undefined
        );
        expect(result).toBe(false);
    });

    it("close sends post command", () => {
        feature.close();

        expect(messaging.sendMessageToHost).toHaveBeenCalledWith(
            expect.any(String),
            expect.objectContaining({
                command: expect.stringContaining("lifecycle:close")
            })
        );
    });
});

function createMessagingMock() {
    return {
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn()
    };
}
