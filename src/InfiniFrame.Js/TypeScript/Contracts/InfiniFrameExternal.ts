// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {BlazorCallback} from "./BlazorInterop";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// noinspection JSDeprecatedSymbols
export interface InfiniFrameExternal extends External {
    receiveMessage?: (callback: BlazorCallback) => void;
    receiveCallback?: (callback: BlazorCallback) => void;
    sendMessage?: (message: string) => void;
    postMessage?: (message: string) => void;
}