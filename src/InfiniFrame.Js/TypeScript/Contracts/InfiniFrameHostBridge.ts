/**
 * Native host bridge contract. Defines the interface for the WebView2/WebKit native host object.
 * @module InfiniFrameHostBridge
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeV1} from "./EnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Interface for the native host object injected by the WebView2 or WebKit runtime.
 * Provides the low-level communication primitives used by the messaging layer.
 */
export interface InfiniFrameHostBridge {
    /**
     * Posts a message (envelope or raw string) to the native host asynchronously.
     * @param envelope - The envelope or string message to send.
     */
    postData(envelope: InteropEnvelopeV1 | string): void;

    /**
     * Registers a callback that receives messages from the native host.
     * @param callback - Function invoked with each incoming message string.
     */
    receiveCallback(callback: (message: string) => void): void;

    /**
     * Sends a message to the host and returns a promise resolving to the host's response.
     * Only available on hosts that support async request/response (e.g. WebView2).
     * @param message - The envelope or string message to send.
     * @returns The host's response as a string.
     */
    getDataAsync?(message: InteropEnvelopeV1 | string): Promise<string>;
}
