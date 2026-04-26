// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InteropEnvelopeV1, ParsedInteropMessage, InteropParseError} from "../Contracts/IInteropEnvelope";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const InteropEnvelopeVersion = 1;
export const InteropMessageMaxSizeBytes = 1024 * 1024;

export function createEnvelope(id: string, data?: unknown, channel?: string): InteropEnvelopeV1 {
    if (!id || id.trim().length === 0) {
        throw new Error("Envelope 'id' is required.");
    }

    return {
        id,
        data,
        version: InteropEnvelopeVersion,
        channel
    };
}

export function createEnvelopeMessage(id: string, data?: unknown, channel?: string): string {
    const envelope = createEnvelope(id, data, channel);

    return JSON.stringify(envelope);
}

export function parseIncomingMessage(message: string): ParsedInteropMessage | InteropParseError {
    if (!message || message.trim().length === 0) {
        return {error: "Message is empty."};
    }

    if (getUtf8ByteCount(message) > InteropMessageMaxSizeBytes) {
        return {error: `Message exceeds max size of ${InteropMessageMaxSizeBytes} bytes.`};
    }

    if (!looksLikeJsonObject(message)) {
        return parseLegacyMessage(message);
    }

    try {
        const parsed = JSON.parse(message) as unknown;
        if (!isObject(parsed)) {
            return {error: "Envelope root must be a JSON object."};
        }

        if (typeof parsed.id !== "string" || parsed.id.trim().length === 0) {
            return {error: "Envelope 'id' is required and must be a string."};
        }

        if (typeof parsed.version !== "number" || !Number.isInteger(parsed.version)) {
            return {error: "Envelope 'version' is required and must be an integer."};
        }

        if (parsed.version !== InteropEnvelopeVersion) {
            return {error: `Unsupported envelope version '${parsed.version}'.`};
        }

        const payload = convertDataToPayload(parsed.data);
        return {
            messageId: parsed.id,
            payload
        };
    } catch {
        return {error: "Envelope JSON is malformed."};
    }
}

function parseLegacyMessage(message: string): ParsedInteropMessage | InteropParseError {
    const separatorIndex = message.indexOf(";");
    const hasSeparator = separatorIndex >= 0;
    const messageId = (hasSeparator ? message.slice(0, separatorIndex) : message).trim();

    if (messageId.length === 0) {
        return {error: "Legacy message has an empty message ID."};
    }

    return {
        messageId,
        payload: hasSeparator ? message.slice(separatorIndex + 1) : undefined,
        isLegacyProtocol: true
    };
}

function convertDataToPayload(data: unknown): string | undefined {
    if (data === null || data === undefined) {
        return undefined;
    }

    if (typeof data === "string") {
        return data;
    }

    return JSON.stringify(data);
}

function looksLikeJsonObject(message: string): boolean {
    return message.replace(/^\s+/, "").startsWith("{");
}

function getUtf8ByteCount(message: string): number {
    return new TextEncoder().encode(message).length;
}

function isObject(value: unknown): value is Record<string, unknown> {
    return typeof value === "object" && value !== null;
}
