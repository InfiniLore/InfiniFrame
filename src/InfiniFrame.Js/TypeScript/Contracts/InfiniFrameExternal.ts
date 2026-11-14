/**
 * External API contract for the window.infiniframe namespace exposed to user code.
 * @module InfiniFrameExternal
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {BlazorCallback} from "./BlazorInterop";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// -----------------------------------------------------------------------------------------------------------------

// noinspection JSDeprecatedSymbols

/**
 * Legacy external interface exposed on `window.external` for backward compatibility with Blazor WebView.
 * Extends the standard `External` interface with InfiniFrame-specific messaging methods.
 */
export interface InfiniFrameExternal extends External {
    /**
     * Registers a callback to receive messages from the host. Deprecated in favor of {@link receiveCallback}.
     * @param callback - Function invoked with each incoming message.
     */
    receiveMessage?: (callback: BlazorCallback) => void;

    /**
     * Registers a callback to receive messages from the host.
     * @param callback - Function invoked with each incoming message.
     */
    receiveCallback?: (callback: BlazorCallback) => void;

    /**
     * Sends a message to the host. Deprecated in favor of {@link postMessage}.
     * @param message - Serialized message string to send.
     */
    sendMessage?: (message: string) => void;

    /**
     * Posts a message to the native host.
     * @param message - Serialized message string to send.
     */
    postMessage?: (message: string) => void;
}
