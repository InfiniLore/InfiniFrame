/**
 * Web messaging envelope protocol v2 contract. Defines the JSON envelope format for messages between JavaScript and C#.
 * @module EnvelopeProtocol
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Version 1 envelope structure for interop messages between JavaScript and the native host.
 * Wraps message data with metadata for routing and identification.
 */
export interface InteropEnvelopeV1 {
    /** Unique identifier for the envelope instance. */
    id: string;
    /** Optional payload data carried by the envelope. */
    data?: unknown;
    /** Protocol version number. */
    version: number;
    /** Command type indicating the direction or intent of the message. */
    command?: InteropEnvelopeCommand;
    /** Optional correlation identifier for request/response matching. */
    requestId?: string;
    /** Optional channel name for multiplexed communication. */
    channel?: string;
}

/**
 * A parsed representation of an incoming interop message after envelope extraction.
 */
export interface ParsedInteropMessage {
    /** The unique message identifier extracted from the envelope. */
    messageId: string;
    /** Optional serialized payload string. */
    payload?: string;
    /** Command type indicating the message direction or intent. */
    command?: InteropEnvelopeCommand;
    /** Optional correlation identifier for request/response matching. */
    requestId?: string;
}

/**
 * Error result returned when an interop message cannot be parsed.
 */
export interface InteropParseError {
    /** Human-readable description of the parse failure. */
    error: string;
}

/**
 * Union of supported envelope command types.
 * - `"Post"`: Fire-and-forget message from JavaScript to host.
 * - `"Get"`: Request/response message expecting a reply from the host.
 */
export type InteropEnvelopeCommand = "Post" | "Get";
