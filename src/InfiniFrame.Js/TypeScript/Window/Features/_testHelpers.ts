/**
 * Shared test helpers for window feature unit tests. Provides mock messaging and feature setup utilities.
 * @module Window/Features/_testHelpers
 */
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
import {vi} from "vitest";

/**
 * Creates a mock messaging object with Vitest spy functions for all messaging methods.
 * @returns A mock messaging object suitable for injecting into feature constructors.
 */
export function createMessagingMock() {
    return {
        sendMessageToHost: vi.fn(),
        getMessageFromHostAsync: vi.fn(),
        getMessageFromHostRawAsync: vi.fn(),
        assignMessageReceivedHandler: vi.fn(),
        unregisterMessageReceivedHandler: vi.fn()
    };
}

/**
 * Sets up a clean test environment by restoring all mocks and creating a fresh messaging mock.
 * @returns The newly created messaging mock object.
 */
export function setupFeature() {
    vi.restoreAllMocks();
    const messaging = createMessagingMock();
    (window as any).infiniframe = {messaging};
    return messaging;
}
