// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InteropEnvelopeV1} from "../Contracts/IInteropEnvelope";
import {InteropEnvelopeVersion} from "./InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function installHostBridge(): void {
    const root: NonNullable<Window["infiniframe"]> = window.infiniframe ?? {};
    const host = (root.host ?? {}) as NonNullable<NonNullable<Window["infiniframe"]>["host"]>;
    const existingPostMessage = host.postMessage;

    host.postMessage = (envelope: InteropEnvelopeV1 | string) => {
        dispatchEnvelopeToHost(envelope, existingPostMessage);
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

    if (existingPostMessage) {
        try {
            existingPostMessage(normalized);
            return;
        } catch (error) {
            console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
        }
    }

    const serializedEnvelope = JSON.stringify(normalized);
    sendViaPlatformTransport(serializedEnvelope);
}

function normalizeEnvelope(envelope: InteropEnvelopeV1): InteropEnvelopeV1 | null {
    if (!envelope || typeof envelope !== "object") {
        console.warn("Host bridge payload must be an envelope object.");
        return null;
    }

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
