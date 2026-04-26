// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame} from "./IInfiniFrame";
import {InteropEnvelopeV1} from "./InteropEnvelope";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {}
declare global {
    // noinspection JSUnusedGlobalSymbols
    interface Window {
        chrome?: {
            webview?: {
                postMessage(message: string): void;
                addEventListener(type: 'message', listener: (event: { data: string }) => void): void;
            };
        };
        infiniframe?: {
            // Managed by InfiniFrame.Native
            host?: {
                postMessage(envelope: InteropEnvelopeV1 | string): void;
                receiveMessage(callback: (message: string) => void): void;
            };
        };
        infiniFrame: IInfiniFrame
    }
}
