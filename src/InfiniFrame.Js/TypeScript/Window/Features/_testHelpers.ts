// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
import {vi} from "vitest";

export function createMessagingMock() {
    return {
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        getMessageFromHostRawAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn()
    };
}

export function setupFeature() {
    vi.restoreAllMocks();
    const messaging = createMessagingMock();
    (window as any).infiniframe = {messaging};
    return messaging;
}
