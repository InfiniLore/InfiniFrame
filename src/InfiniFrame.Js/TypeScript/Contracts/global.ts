// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame} from "./IInfiniFrame";
import {InteropEnvelopeV1} from "./IInteropEnvelope";
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
            host?: {
                postMessage(envelope: InteropEnvelopeV1 | string): void;
            };
        };
        infiniFrame: IInfiniFrame
    }

    // noinspection JSUnusedGlobalSymbols
    interface External {
        sendMessage?(message: string): void;

        receiveMessage?: (message: string) => void;
    }
}
