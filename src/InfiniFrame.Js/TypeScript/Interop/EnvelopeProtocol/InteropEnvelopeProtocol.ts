/**
 * @file Envelope protocol implementation. Creates and parses the v2 JSON envelope format for web messaging.
 */
// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeCommand, InteropEnvelopeV1, InteropParseError, ParsedInteropMessage} from "../../Contracts";
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {SendToHostMessageIds} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/** The current envelope protocol version. */
export const InteropEnvelopeVersion = 2;

/** Maximum allowed message size in bytes (1 MiB). */
export const InteropMessageMaxSizeBytes = 1024 * 1024;

/** The command identifier for fire-and-forget (post) messages. */
export const InteropPostCommand: InteropEnvelopeCommand = "Post";

/** The command identifier for request/response (get) messages. */
export const InteropGetCommand: InteropEnvelopeCommand = "Get";

/**
 * Creates a new {@link InteropEnvelopeV1} object with the given fields, applying defaults and validation.
 *
 * @param id - The message identifier. Must be a non-empty string.
 * @param data - Optional payload data to include in the envelope.
 * @param channel - Optional channel name for routing the message.
 * @param command - The envelope command type. Defaults to {@link InteropPostCommand}.
 * @param requestId - Optional request ID for correlating get-style request/response pairs.
 * @returns A fully constructed {@link InteropEnvelopeV1}.
 * @throws {Error} If the provided `id` is empty or whitespace-only.
 */
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

/**
 * Creates a get-style envelope for requesting data from the host.
 *
 * @param command - The command name identifying the data to retrieve.
 * @param args - Optional arguments to pass along with the request.
 * @returns A {@link InteropEnvelopeV1} configured for a get request.
 */
export function createGetEnvelope(
    command: string,
    args?: unknown,
): InteropEnvelopeV1 {
    return createEnvelope(SendToHostMessageIds.getRequest, {command, args}, undefined, InteropGetCommand);
}

/**
 * Creates an envelope and serializes it to a JSON string.
 *
 * @param id - The message identifier. Must be a non-empty string.
 * @param data - Optional payload data to include in the envelope.
 * @param channel - Optional channel name for routing the message.
 * @param command - The envelope command type. Defaults to {@link InteropPostCommand}.
 * @param requestId - Optional request ID for correlating get-style request/response pairs.
 * @returns The JSON-serialized envelope string.
 * @throws {Error} If the provided `id` is empty or whitespace-only.
 */
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

/**
 * Parses an incoming JSON message string into a structured {@link ParsedInteropMessage},
 * or returns an {@link InteropParseError} if the message is invalid.
 *
 * @param message - The raw JSON string received from the host.
 * @returns A parsed message object or a parse error describing what went wrong.
 */
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

/**
 * Converts the `data` field of an envelope into a string payload suitable for the parsed message.
 *
 * @param data - The raw data field from the envelope.
 * @returns The data as a string, or `undefined` if the data is null/undefined.
 */
function convertDataToPayload(data: unknown): string | undefined {
    if (data === null || data === undefined) {
        return undefined;
    }

    if (typeof data === "string") {
        return data;
    }

    return JSON.stringify(data);
}

/**
 * Returns the UTF-8 byte length of a string.
 *
 * @param message - The string to measure.
 * @returns The number of UTF-8 bytes required to encode the string.
 */
function getUtf8ByteCount(message: string): number {
    return new TextEncoder().encode(message).length;
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
 * Type guard that checks whether a value is a supported envelope command.
 *
 * @param command - The value to check.
 * @returns `true` if the value is either {@link InteropPostCommand} or {@link InteropGetCommand}.
 */
function isSupportedCommand(command: unknown): command is InteropEnvelopeCommand {
    return command === InteropPostCommand || command === InteropGetCommand;
}
