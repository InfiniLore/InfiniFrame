// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniWindow} from "./IInfiniWindow";
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
        infiniWindow: IInfiniWindow
    }

    // noinspection JSUnusedGlobalSymbols
    interface External {
        sendMessage?(message: string): void;

        receiveMessage?: (message: string) => void;
    }
}