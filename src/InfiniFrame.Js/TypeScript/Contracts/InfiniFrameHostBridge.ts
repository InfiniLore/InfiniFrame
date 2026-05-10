// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeV1} from "./EnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameHostBridge {
    postData(envelope: InteropEnvelopeV1 | string): void;

    receiveCallback(callback: (message: string) => void): void;

    getDataAsync?(message: InteropEnvelopeV1 | string): Promise<string>;
}