/**
 * Shared test helpers for window feature unit tests. Provides mock messaging and feature setup utilities.
 * @module Window/Features/_testHelpers
 */
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
import {vi} from "vitest";
import {getTestWindow} from "../../_testHelpers";

/**
 * Creates a mock messaging object with Vitest spy functions for all messaging methods.
 * @returns A mock messaging object suitable for injecting into feature constructors.
 */
export function createMessagingMock() {
    return {
        ready: Promise.resolve(),
        isReady: true,
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        getMessageFromHostRawAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn(),
    };
}

/**
 * Sets up a clean test environment by restoring all mocks and creating a fresh messaging mock.
 * @returns The newly created messaging mock object.
 */
export function setupFeature() {
    vi.restoreAllMocks();
    const messaging = createMessagingMock();
    const testWindow = getTestWindow();
    testWindow.infiniframe = {messaging};
    return messaging;
}
