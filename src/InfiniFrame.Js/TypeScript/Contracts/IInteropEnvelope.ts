// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InteropEnvelopeV1 {
    id: string;
    data?: unknown;
    version: number;
}

export interface ParsedInteropMessage {
    messageId: string;
    payload?: string;
}

export interface ParseError {
    error: string;
}
