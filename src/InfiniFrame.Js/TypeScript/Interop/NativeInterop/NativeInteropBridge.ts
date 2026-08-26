/**
 * @file Native interop bridge. Installs the window.infiniframe.host object that communicates with the native WebView host.
 */
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameHostBridge, InfiniFrameSetup, InteropEnvelopeCommand, InteropEnvelopeV1} from "../../Contracts";
import {
    InteropEnvelopeVersion,
    InteropGetCommand,
    InteropPostCommand,
    parseIncomingMessage
} from "../EnvelopeProtocol/InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const GetMessageResponseId = "__infiniframe:get:response";
const GetMessageTimeoutMs = 10_000;

const receiveCallbacks = new Set<(message: string) => void>();
let receiveBridgeAttached = false;

/**
 * Resets the internal bridge state, clearing all registered receive callbacks and the bridge-attached flag.
 *
 * @returns {void}
 */
export function resetNativeInteropBridgeState(): void {
    receiveCallbacks.clear();
    receiveBridgeAttached = false;
}

/**
 * Installs the native interop bridge on `window.infiniframe.host`, wiring up {@link InfiniFrameHostBridge.postData},
 * {@link InfiniFrameHostBridge.receiveCallback}, and {@link InfiniFrameHostBridge.getDataAsync}.
 *
 * @param setup - The setup guard that tracks which initializations have already been performed.
 * @returns {void}
 */
export function installNativeInteropBridge(setup: InfiniFrameSetup): void {
    if (setup.nativeInteropBridgeInitialized) return;
    setup.nativeInteropBridgeInitialized = true;

    window.infiniframe = window.infiniframe ?? {} as Window["infiniframe"];
    const host = (window.infiniframe.host ?? {}) as InfiniFrameHostBridge;
    const existingPostData = host.postData;
    const existingReceiveCallback = host.receiveCallback;
    const existingGetData = host.getDataAsync;

    host.postData = (envelope: InteropEnvelopeV1 | string) => {
        dispatchEnvelopeToHost(envelope, existingPostData);
    };
    host.receiveCallback = (callback: (message: string) => void) => {
        registerWebMessageReceiver(callback, existingReceiveCallback);
    };
    host.getDataAsync = (message: InteropEnvelopeV1 | string) => {
        return requestMessageFromHost(message, host, existingGetData, existingReceiveCallback);
    };

    window.infiniframe.host = host;
}

/**
 * Dispatches an envelope or raw string message to the native host via the existing bridge or platform adapter.
 *
 * @param envelope - The envelope object or raw string to send.
 * @param existingPostData - Optional previously-installed post-data handler to try first.
 * @returns {void}
 */
function dispatchEnvelopeToHost(
    envelope: InteropEnvelopeV1 | string,
    existingPostData?: ((envelope: InteropEnvelopeV1 | string) => void)
): void {
    if (typeof envelope === "string") {
        const rawMessage = envelope.trim();
        if (rawMessage.length === 0) {
            console.warn("Ignoring empty host bridge payload.");
            return;
        }

        if (existingPostData) {
            try {
                existingPostData(rawMessage);
                return;
            } catch (error) {
                console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
            }
        }

        postToPlatform(rawMessage);
        return;
    }

    const normalized = normalizeEnvelope(envelope);
    if (!normalized) {
        return;
    }

    const serializedEnvelope = JSON.stringify(normalized);

    if (existingPostData) {
        try {
            // Prefer the string contract for host adapters that only accept raw messages.
            existingPostData(serializedEnvelope);
            return;
        } catch (error) {
            try {
                // Backward compatibility for adapters that still expect an envelope object.
                existingPostData(normalized);
                return;
            } catch {
                console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
            }
        }
    }

    postToPlatform(serializedEnvelope);
}

/**
 * Sends a get-style message to the host and returns a promise that resolves with the host response.
 *
 * @param message - The envelope object or string message to request from the host.
 * @param host - The host bridge instance used to post the request.
 * @param existingGetData - Optional previously-installed get-data handler to try first.
 * @param existingReceiveCallback - Optional previously-installed receive callback handler to try first.
 * @returns A promise that resolves with the host response string.
 */
function requestMessageFromHost(
    message: InteropEnvelopeV1 | string,
    host: InfiniFrameHostBridge,
    existingGetData?: ((message: InteropEnvelopeV1 | string) => Promise<string> | string),
    existingReceiveCallback?: (callback: (message: string) => void) => void
): Promise<string> {
    const normalizedMessage = normalizeGetMessageInput(message);
    if (!normalizedMessage) {
        return Promise.reject(new Error("Host getDataAsync payload is invalid."));
    }

    if (existingGetData) {
        try {
            const existingResult = existingGetData(normalizedMessage);
            if (existingResult && typeof (existingResult as Promise<string>).then === "function") {
                return existingResult as Promise<string>;
            }

            return Promise.resolve(String(existingResult ?? ""));
        } catch (error) {
            console.warn("Existing InfiniFrame getDataAsync bridge failed. Falling back to request/response transport.", error);
        }
    }

    const requestId = createRequestId();

    return new Promise<string>((resolve, reject) => {
        const timeout = window.setTimeout(() => {
            unregisterWebMessageReceiver(responseCallback);
            reject(new Error("Timed out waiting for getDataAsync response from host."));
        }, GetMessageTimeoutMs);

        const responseCallback = (rawMessage: string) => {
            const parsed = parseIncomingMessage(rawMessage);
            if ("error" in parsed || parsed.messageId !== GetMessageResponseId || !parsed.payload) {
                return;
            }

            let payload: unknown;
            try {
                payload = JSON.parse(parsed.payload);
            } catch {
                return;
            }

            if (!isGetMessageResponsePayload(payload) || payload.requestId !== requestId) {
                return;
            }

            window.clearTimeout(timeout);
            unregisterWebMessageReceiver(responseCallback);

            if (payload.success) {
                resolve(payload.data ?? "");
                return;
            }

            reject(new Error(payload.error ?? "Host getDataAsync failed."));
        };

        registerWebMessageReceiver(responseCallback, existingReceiveCallback);
        const requestEnvelope = createGetRequestEnvelope(normalizedMessage, requestId);

        if (!requestEnvelope) {
            window.clearTimeout(timeout);
            unregisterWebMessageReceiver(responseCallback);
            reject(new Error("Host getDataAsync payload is invalid."));
            return;
        }

        host.postData?.(requestEnvelope);
    });
}

/**
 * Creates a get-request envelope from a normalized message and request ID.
 *
 * @param normalizedMessage - The normalized envelope or string message.
 * @param requestId - The unique request identifier to include in the envelope.
 * @returns A normalized {@link InteropEnvelopeV1} for a get request, or null if the input is invalid.
 */
function createGetRequestEnvelope(normalizedMessage: InteropEnvelopeV1 | string, requestId: string): InteropEnvelopeV1 | null {
    if (typeof normalizedMessage !== "string") {
        return normalizeEnvelope(normalizedMessage, InteropGetCommand, requestId);
    }

    try {
        const parsed = JSON.parse(normalizedMessage) as unknown;
        if (isObject(parsed)) {
            return normalizeEnvelope(parsed as unknown as InteropEnvelopeV1, InteropGetCommand, requestId);
        }
    } catch {
        // A plain string is treated as the message id for a get request.
    }

    return normalizeEnvelope({id: normalizedMessage, version: InteropEnvelopeVersion}, InteropGetCommand, requestId);
}

/**
 * Normalizes a get-message input by trimming strings and validating envelope objects.
 *
 * @param message - The raw message input (string or envelope object).
 * @returns The trimmed string or normalized envelope, or null if the input is invalid.
 */
function normalizeGetMessageInput(message: InteropEnvelopeV1 | string): InteropEnvelopeV1 | string | null {
    if (typeof message === "string") {
        const trimmed = message.trim();
        if (trimmed.length === 0) {
            return null;
        }

        return trimmed;
    }

    const normalizedEnvelope = normalizeEnvelope(message);
    if (!normalizedEnvelope) {
        return null;
    }

    return normalizedEnvelope;
}

/**
 * Generates a unique request ID using a timestamp and random hex bytes.
 *
 * @returns A unique request ID string prefixed with `if_req_`.
 */
function createRequestId(): string {
    const randomBytes = new Uint8Array(16);
    crypto.getRandomValues(randomBytes);
    const randomHex = Array.from(randomBytes, b => b.toString(16).padStart(2, '0')).join('');
    return `if_req_${Date.now().toString(36)}_${randomHex}`;
}

/**
 * Normalizes an envelope object by filling in missing fields and validating required properties.
 *
 * @param envelope - The raw envelope object to normalize.
 * @param command - Optional command to override the envelope's command field.
 * @param requestId - Optional request ID to override the envelope's requestId field.
 * @returns A fully normalized {@link InteropEnvelopeV1}, or null if the envelope is invalid.
 */
function normalizeEnvelope(
    envelope: InteropEnvelopeV1,
    command?: InteropEnvelopeCommand,
    requestId?: string
): InteropEnvelopeV1 | null {
    if (!envelope || typeof envelope !== "object") {
        console.warn("Host bridge payload must be an envelope object.");
        return null;
    }

    // noinspection SuspiciousTypeOfGuard
    if (typeof envelope.id !== "string" || envelope.id.trim().length === 0) {
        console.warn("Host bridge envelope requires a non-empty 'id'.");
        return null;
    }

    const normalized: InteropEnvelopeV1 = {
        id: envelope.id,
        command: command ?? envelope.command ?? InteropPostCommand,
        requestId: requestId ?? envelope.requestId,
        data: envelope.data,
        version: InteropEnvelopeVersion
    };

    // noinspection SuspiciousTypeOfGuard
    if (envelope.channel !== undefined && typeof envelope.channel === "string" && envelope.channel.trim().length > 0) {
        normalized.channel = envelope.channel;
    }

    return normalized;
}

/**
 * Registers a callback to receive messages from the native host.
 *
 * @param callback - The callback function to invoke when a message is received.
 * @param existingReceiveCallback - Optional previously-installed receive callback handler to try first.
 * @returns {void}
 */
function registerWebMessageReceiver(
    callback: (message: string) => void,
    existingReceiveCallback?: (callback: (message: string) => void) => void
): void {
    receiveCallbacks.add(callback);
    attachReceiveBridgeOnce(existingReceiveCallback);
}

/**
 * Unregisters a previously registered message receive callback.
 *
 * @param callback - The callback function to remove from the receive set.
 * @returns {void}
 */
function unregisterWebMessageReceiver(callback: (message: string) => void): void {
    receiveCallbacks.delete(callback);
}

/**
 * Attaches the platform-specific receive bridge exactly once, dispatching messages to all registered callbacks.
 *
 * @param existingReceiveCallback - Optional previously-installed receive callback handler to try first.
 * @returns {void}
 */
function attachReceiveBridgeOnce(existingReceiveCallback?: (callback: (message: string) => void) => void): void {
    if (receiveBridgeAttached) {
        return;
    }

    const dispatch = (message: string) => {
        for (const callback of receiveCallbacks) {
            callback(message);
        }
    };

    if (existingReceiveCallback) {
        try {
            existingReceiveCallback(dispatch);
            receiveBridgeAttached = true;
            return;
        } catch (error) {
            console.warn("Existing InfiniFrame host receive bridge failed. Falling back to platform adapters.", error);
        }
    }

    if (window.chrome?.webview?.addEventListener) {
        window.chrome.webview.addEventListener("message", event => dispatch(event.data));
        receiveBridgeAttached = true;
        return;
    }

    if (window.webkit?.messageHandlers?.infiniFrameInterop) {
        Object.defineProperty(window, '__infiniframe_dispatch', {
            value: dispatch,
            writable: false,
            configurable: false,
            enumerable: false
        });
        receiveBridgeAttached = true;
        return;
    }

    console.warn("Receive message registration failed. No supported host receive transport was found.");
}

/**
 * Posts a raw string message to the native host using the platform-specific transport.
 *
 * @param message - The serialized message string to send.
 * @returns {void}
 */
function postToPlatform(message: string): void {
    if (window.chrome?.webview?.postMessage) {
        window.chrome.webview.postMessage(message);
        return;
    }

    if (window.webkit?.messageHandlers?.infiniFrameInterop?.postMessage) {
        window.webkit.messageHandlers.infiniFrameInterop.postMessage(message);
        return;
    }

    console.warn("[InfiniFrame] No native bridge available:", message);
}

/**
 * Type guard that checks whether a value is a non-null object.
 *
 * @param value - The value to check.
 * @returns `true` if the value is a non-null object.
 */
function isObject(value: unknown): value is Record<string, unknown> {
    return typeof value === "object" && value !== null;
}

/**
 * Type guard that checks whether a value has the shape of a get-message response payload.
 *
 * @param value - The value to check.
 * @returns `true` if the value matches the expected response payload shape.
 */
function isGetMessageResponsePayload(value: unknown): value is {
    requestId: string;
    success: boolean;
    data?: string;
    error?: string;
} {
    return isObject(value)
        && typeof value.requestId === "string"
        && typeof value.success === "boolean"
        && (value.data === undefined || typeof value.data === "string")
        && (value.error === undefined || typeof value.error === "string");
}
