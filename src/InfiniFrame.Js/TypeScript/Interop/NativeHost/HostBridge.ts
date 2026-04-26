// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InteropEnvelopeV1} from "../../Contracts/IInteropEnvelope";
import {InteropEnvelopeVersion} from "../EnvelopeProtocol/InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function installHostBridge(): void {
    const root: NonNullable<Window["infiniframe"]> = window.infiniframe ?? {};
    const host = (root.host ?? {}) as NonNullable<NonNullable<Window["infiniframe"]>["host"]>;
    const existingPostMessage = host.postMessage;
    const existingReceiveMessage = host.receiveMessage;

    host.postMessage = (envelope: InteropEnvelopeV1 | string) => {
        dispatchEnvelopeToHost(envelope, existingPostMessage);
    };
    host.receiveMessage = (callback: (message: string) => void) => {
        registerWebMessageReceiver(callback, existingReceiveMessage);
    };

    root.host = host;
    window.infiniframe = root;
}

function dispatchEnvelopeToHost(
    envelope: InteropEnvelopeV1 | string,
    existingPostMessage?: ((envelope: InteropEnvelopeV1 | string) => void)
): void {
    if (typeof envelope === "string") {
        const rawMessage = envelope.trim();
        if (rawMessage.length === 0) {
            console.warn("Ignoring empty host bridge payload.");
            return;
        }

        if (existingPostMessage) {
            try {
                existingPostMessage(rawMessage);
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

    if (existingPostMessage) {
        try {
            // Prefer the string contract for host adapters that only accept raw messages.
            existingPostMessage(serializedEnvelope);
            return;
        } catch (error) {
            try {
                // Backward compatibility for adapters that still expect an envelope object.
                existingPostMessage(normalized);
                return;
            } catch {
                console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
            }
        }
    }

    sendViaPlatformTransport(serializedEnvelope);
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
    existingReceiveMessage?: (callback: (message: string) => void) => void
): void {
    if (existingReceiveMessage) {
        try {
            existingReceiveMessage(callback);
            return;
        } catch (error) {
            console.warn("Existing InfiniFrame host receive bridge failed. Falling back to platform adapters.", error);
        }
    }

    if (window.chrome?.webview) {
        window.chrome.webview.addEventListener("message", (event) => {
            callback(event.data);
        });
        return;
    }

    console.warn("Receive message registration failed. No supported host receive transport was found.");
}
