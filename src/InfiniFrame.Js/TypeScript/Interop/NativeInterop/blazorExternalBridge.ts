// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {BlazorCallback, InfiniFrameExternal} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function initWindowExternalBridge(): void {
    const external = window.external as InfiniFrameExternal;
    window.__blazorCallbacks = window.__blazorCallbacks ?? [];

    external.receiveMessage = (callback: BlazorCallback): void => {
        window.__blazorCallbacks!.push(callback);
    };

    external.receiveCallback = external.receiveMessage;

    external.sendMessage = (message: string): void => {
        if (!window.__infiniframe?.host?.postData) {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }

        window.__infiniframe.host.postData(message);
    };

    external.postMessage = external.sendMessage;

    if (!window.__blazorDispatchHooked) {
        window.__blazorDispatchHooked = true;

        window.__infiniframe?.host?.receiveCallback((message: string) => {
            for (const callback of window.__blazorCallbacks ?? []) {
                try {
                    callback(message);
                } catch {
                    // Blazor callbacks are user code. Keep the host dispatch path alive.
                }
            }
        });
    }
}
