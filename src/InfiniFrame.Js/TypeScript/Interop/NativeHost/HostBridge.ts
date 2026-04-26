// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InteropEnvelopeV1} from "../../Contracts";
import {InteropEnvelopeVersion, parseIncomingMessage} from "../EnvelopeProtocol/InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const GetMessageRequestId = "__infiniframe:get:request";
const GetMessageResponseId = "__infiniframe:get:response";
const GetMessageTimeoutMs = 10_000;

type ReceiveCallback = (message: string) => void;
const receiveCallbacks = new Set<ReceiveCallback>();
let receiveBridgeAttached = false;

export function installHostBridge(): void {
    const root: NonNullable<Window["infiniframe"]> = window.infiniframe ?? {};
    const host = (root.host ?? {}) as NonNullable<NonNullable<Window["infiniframe"]>["host"]>;
    const existingPostData = host.postData;
    const existingReceiveCallback = host.receiveCallback;
    const existingGetData = host.getData;

    host.postData = (envelope: InteropEnvelopeV1 | string) => {
        dispatchEnvelopeToHost(envelope, existingPostData);
    };
    host.receiveCallback = (callback: (message: string) => void) => {
        registerWebMessageReceiver(callback, existingReceiveCallback);
    };
    host.getData = (message: InteropEnvelopeV1 | string) => {
        return requestMessageFromHost(message, host, existingGetData, existingReceiveCallback);
    };

    root.host = host;
    window.infiniframe = root;
}

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

        sendViaPlatformTransport(rawMessage);
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

    sendViaPlatformTransport(serializedEnvelope);
}

function requestMessageFromHost(
    message: InteropEnvelopeV1 | string,
    host: NonNullable<NonNullable<Window["infiniframe"]>["host"]>,
    existingGetData?: ((message: InteropEnvelopeV1 | string) => Promise<string> | string),
    existingReceiveCallback?: (callback: (message: string) => void) => void
): Promise<string> {
    const normalizedMessage = normalizeGetMessageInput(message);
    if (!normalizedMessage) {
        return Promise.reject(new Error("Host getData payload is invalid."));
    }

    if (existingGetData) {
        try {
            const existingResult = existingGetData(normalizedMessage);
            if (existingResult && typeof (existingResult as Promise<string>).then === "function") {
                return existingResult as Promise<string>;
            }

            return Promise.resolve(String(existingResult ?? ""));
        } catch (error) {
            console.warn("Existing InfiniFrame getData bridge failed. Falling back to request/response transport.", error);
        }
    }

    const requestId = createRequestId();

    return new Promise<string>((resolve, reject) => {
        const timeout = window.setTimeout(() => {
            unregisterWebMessageReceiver(responseCallback);
            reject(new Error("Timed out waiting for getData response from host."));
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

            reject(new Error(payload.error ?? "Host getData failed."));
        };

        registerWebMessageReceiver(responseCallback, existingReceiveCallback);
        host.postData?.({
            id: GetMessageRequestId,
            data: {
                requestId,
                message: normalizedMessage
            },
            version: InteropEnvelopeVersion
        });
    });
}

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

function createRequestId(): string {
    return `if_req_${Date.now().toString(36)}_${Math.random().toString(36).slice(2, 10)}`;
}

function normalizeEnvelope(envelope: InteropEnvelopeV1): InteropEnvelopeV1 | null {
    if (!envelope || typeof envelope !== "object") {
        console.warn("Host bridge payload must be an envelope object.");
        return null;
    }

    // noinspection SuspiciousTypeOfGuard
    if (typeof envelope.id !== "string" || envelope.id.trim().length === 0) {
        console.warn("Host bridge envelope requires a non-empty 'id'.");
        return null;
    }

    const version = Number.isInteger(envelope.version)
        ? envelope.version
        : InteropEnvelopeVersion;

    const normalized: InteropEnvelopeV1 = {
        id: envelope.id,
        data: envelope.data,
        version
    };

    // noinspection SuspiciousTypeOfGuard
    if (envelope.channel !== undefined && typeof envelope.channel === "string" && envelope.channel.trim().length > 0) {
        normalized.channel = envelope.channel;
    }

    return normalized;
}

function sendViaPlatformTransport(message: string): void {
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage(message);
        return;
    }

    console.warn("Message to host failed. No supported host transport was found.");
}

function registerWebMessageReceiver(
    callback: (message: string) => void,
    existingReceiveCallback?: (callback: (message: string) => void) => void
): void {
    receiveCallbacks.add(callback);
    attachReceiveBridgeOnce(existingReceiveCallback);
}

function unregisterWebMessageReceiver(callback: ReceiveCallback): void {
    receiveCallbacks.delete(callback);
}

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

    if (window.chrome?.webview) {
        window.chrome.webview.addEventListener("message", (event) => {
            dispatch(event.data);
        });
        receiveBridgeAttached = true;
        return;
    }

    console.warn("Receive message registration failed. No supported host receive transport was found.");
}

function isObject(value: unknown): value is Record<string, unknown> {
    return typeof value === "object" && value !== null;
}

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
