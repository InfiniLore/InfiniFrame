// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame} from "./IInfiniFrame";
import {InteropEnvelopeV1} from "./EnvelopeProtocol";
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
                postData(envelope: InteropEnvelopeV1 | string): void;
                receiveCallback(callback: (message: string) => void): void;
                getData?(message: InteropEnvelopeV1 | string): Promise<string>;
            };
        };
        infiniFrame: IInfiniFrame
    }
}
