// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

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
            };
        };
        
        setPointerCapture(element: Element, pointerId: number): void;
        releasePointerCapture(element: Element, pointerId: number): void;
    }
    // noinspection JSUnusedGlobalSymbols
    interface External {
        sendMessage?(message: string): void;
    }
}