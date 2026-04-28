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
        __infiniframe: {
           host: {
                postData(envelope: InteropEnvelopeV1 | string): void;
                receiveCallback(callback: (message: string) => void): void;
                getDataAsync?(message: InteropEnvelopeV1 | string): Promise<string>;
            };
        };
    }
}
