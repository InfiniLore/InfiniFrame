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
        infiniframe : IInfiniFrame;
        
        // Managed by InfiniFrame.Native
        __infiniframe?: {
           host?: {
                postData(envelope: InteropEnvelopeV1 | string): void;
                receiveCallback(callback: (message: string) => void): void;
                getDataAsync?(message: InteropEnvelopeV1 | string): Promise<string>;
            };
        };
        __dispatchMessageCallback?: (message: string) => void;
        chrome?: {
            webview?: {
                postMessage(message: string): void;
                addEventListener(type: "message", listener: (event: { data: string }) => void): void;
            };
        };
        webkit?: {
            messageHandlers?: {
                infiniFrameInterop?: {
                    postMessage(message: string): void;
                };
            };
        };
    }
}
