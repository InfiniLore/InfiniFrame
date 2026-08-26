/**
 * Host messaging contracts. Defines message ID constants and callback types for the native messaging bridge.
 * @module InfiniFrameHostMessaging
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeV1} from "./EnvelopeProtocol";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const infiniframe: string = "__infiniframe";
const window: string = "window";
const features: string = "features";

const windowFeaturePrefix: string = `${infiniframe}:${window}:${features}`;

/**
 * Message IDs used when sending messages from JavaScript to the native host.
 */
export const SendToHostMessageIds = {
    /** Request a value from the host via the get/response pattern. */
    getRequest: `${infiniframe}:get`,
    /** Request the window enter fullscreen mode. */
    fullscreenEnter: `${infiniframe}:fullscreen:enter`,
    /** Request the window exit fullscreen mode. */
    fullscreenExit: `${infiniframe}:fullscreen:exit`,
    /** Request the host to open a URL in the system default browser. */
    openExternalLink: `${infiniframe}:open:external`,
    /** Request the host to close the window. */
    windowClose: `${infiniframe}:window:close`,
    /** Signal that the JavaScript runtime is ready. */
    ready: `${infiniframe}:ready`,
    /** Prefix for window feature request messages. */
    windowFeatureRequest: windowFeaturePrefix,
    /** Acknowledge receipt of a web message from the host. */
    webMessageAckResponse: `${infiniframe}:message:ack:response`,
}

/**
 * Message IDs used when receiving messages from the native host that require a response.
 */
export const GetMessageFromHostMessageIds = {
    /** Prefix for window feature messages originating from the host. */
    windowFeaturePrefix,
}

/**
 * Message IDs used when receiving fire-and-forget messages from the native host.
 */
export const ReceiveFromHostMessageIds = {
    /** Register a handler for external link open requests. */
    registerOpenExternal: `${infiniframe}:register:open:external`,
    /** Register a handler for fullscreen state change notifications. */
    registerFullscreenChange: `${infiniframe}:register:fullscreen:change`,
    /** Register a handler for window title change notifications. */
    registerTitleChange: `${infiniframe}:register:title:change`,
    /** Register a handler for window close notifications. */
    registerWindowClose: `${infiniframe}:register:window:close`,
    /** Acknowledge that the host is ready. */
    readyAck: `${infiniframe}:ready:ack`,
    /** Response to a previously sent get request. */
    getMessageResponse: `${infiniframe}:get:response`,
    /** Request to acknowledge receipt of a web message. */
    webMessageAckRequest: `${infiniframe}:message:ack:request`,
    /** Enable or disable the browser context menu. */
    setContextMenuEnabled: `${infiniframe}:browser:setContextMenuEnabled`,
    /** Enable or disable browser zoom functionality. */
    setZoomEnabled: `${infiniframe}:browser:setZoomEnabled`,
    /** Enable or disable browser keyboard shortcuts. */
    setBrowserShortcutsEnabled: `${infiniframe}:browser:setBrowserShortcutsEnabled`,
}

/**
 * Union type of all message ID string values sent to the host.
 */
export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];

/**
 * Callback signature for message received handlers.
 * Receives an optional serialized data payload.
 */
export type MessageCallback = (data?: string) => void;

/**
 * Messaging interface for bidirectional communication between JavaScript and the native host.
 */
export interface InfiniFrameHostMessaging {
    /** Promise that resolves when the host messaging layer is initialized and ready. */
    readonly ready: Promise<void>;
    /** Whether the host messaging layer has finished initializing. */
    readonly isReady: boolean;

    /**
     * Sends a message to the native host.
     * @param id - Message ID identifying the message type.
     * @param data - Optional payload to send with the message.
     */
    sendMessageToHost(id: SendToHostMessageId | string, data?: unknown): void;

    /**
     * Sends a raw envelope or string message to the host and awaits a string response.
     * @param message - The envelope or string to send.
     * @returns The host's response as a string.
     */
    getMessageFromHostRawAsync(message: InteropEnvelopeV1 | string): Promise<string>;

    /**
     * Sends a named message with optional arguments to the host and awaits a string response.
     * @param message - The message name or ID.
     * @param args - Optional arguments to include.
     * @returns The host's response as a string.
     */
    getMessageFromHostAsync(message: string, args?: any): Promise<string>;

    /**
     * Registers a callback for a specific incoming message ID.
     * @param messageId - The message ID to listen for.
     * @param callback - Function invoked when the message is received.
     */
    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    /**
     * Removes a previously registered callback for a specific message ID.
     * @param messageId - The message ID to stop listening for.
     */
    unregisterMessageReceivedHandler(messageId: string): void;
}
