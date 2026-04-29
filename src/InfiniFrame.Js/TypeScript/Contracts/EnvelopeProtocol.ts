// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InteropEnvelopeV1 {
    id: string;
    data?: unknown;
    version: number;
    command?: InteropEnvelopeCommand;
    requestId?: string;
    channel?: string;
}

export interface ParsedInteropMessage {
    messageId: string;
    payload?: string;
    command?: InteropEnvelopeCommand;
    requestId?: string;
}

export interface InteropParseError {
    error: string;
}

export type InteropEnvelopeCommand = "Post" | "Get";
