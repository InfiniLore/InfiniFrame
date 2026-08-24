// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeCommand, InteropEnvelopeV1, InteropParseError, ParsedInteropMessage} from "../../Contracts";
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const InteropEnvelopeVersion = 2;
export const InteropMessageMaxSizeBytes = 1024 * 1024;
export const InteropPostCommand: InteropEnvelopeCommand = "Post";
export const InteropGetCommand: InteropEnvelopeCommand = "Get";

export function createEnvelope(
    id: string,
    data?: unknown,
    channel?: string,
    command: InteropEnvelopeCommand = InteropPostCommand,
    requestId?: string
): InteropEnvelopeV1 {
    if (!id || id.trim().length === 0) {
        throw new Error("Envelope 'id' is required.");
    }

    return {
        id,
        command,
        requestId,
        data,
        version: InteropEnvelopeVersion,
        channel
    };
}

export function createGetEnvelope(
    command: string,
    args?: unknown,
): InteropEnvelopeV1 {
    return createEnvelope(SendToHostMessageIds.getRequest, {command, args}, undefined, InteropGetCommand);
}

export function createEnvelopeMessage(
    id: string,
    data?: unknown,
    channel?: string,
    command: InteropEnvelopeCommand = InteropPostCommand,
    requestId?: string
): string {
    const envelope = createEnvelope(id, data, channel, command, requestId);

    return JSON.stringify(envelope);
}

export function parseIncomingMessage(message: string): ParsedInteropMessage | InteropParseError {
    if (!message || message.trim().length === 0) {
        return {error: "Message is empty."};
    }

    if (getUtf8ByteCount(message) > InteropMessageMaxSizeBytes) {
        return {error: `Message exceeds max size of ${InteropMessageMaxSizeBytes} bytes.`};
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

        if (!isSupportedCommand(parsed.command)) {
            return {error: "Envelope 'command' must be 'Post' or 'Get'."};
        }

        if (parsed.requestId !== undefined && typeof parsed.requestId !== "string") {
            return {error: "Envelope 'requestId' must be a string."};
        }

        return {
            messageId: parsed.id,
            payload,
            command: parsed.command,
            requestId: parsed.requestId
        };
    } catch {
        return {error: "Envelope JSON is malformed."};
    }
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

function getUtf8ByteCount(message: string): number {
    return new TextEncoder().encode(message).length;
}

function isObject(value: unknown): value is Record<string, unknown> {
    return typeof value === "object" && value !== null;
}

function isSupportedCommand(command: unknown): command is InteropEnvelopeCommand {
    return command === InteropPostCommand || command === InteropGetCommand;
}
