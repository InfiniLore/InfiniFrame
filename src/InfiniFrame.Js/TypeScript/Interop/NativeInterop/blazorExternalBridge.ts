/**
 * @file Blazor external bridge. Registers external link handling to open target='_blank' links in the default browser.
 */
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {BlazorCallback, InfiniFrameExternal, InfiniFrameSetup} from "../../Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Initializes the `window.external` bridge, wiring up Blazor message callbacks, send/receive helpers,
 * and dispatching incoming host messages to all registered Blazor callbacks.
 *
 * @param setup - The setup guard that tracks which initializations have already been performed.
 * @returns {void}
 */
export function initWindowExternalBridge(setup: InfiniFrameSetup): void {
    if (setup.windowExternalBridgeInitialized) return;
    setup.windowExternalBridgeInitialized = true;

    const external = ensureWindowExternal();
    window.infiniframe = window.infiniframe ?? {} as Window["infiniframe"];
    const callbacks: BlazorCallback[] = [];
    (window.infiniframe as unknown as Record<string, unknown>).__blazorCallbacks = callbacks;

    external.receiveMessage = (callback: BlazorCallback): void => {
        callbacks.push(callback);
    };

    external.receiveCallback = external.receiveMessage;

    external.sendMessage = (message: string): void => {
        if (!window.infiniframe?.host?.postData) {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }

        window.infiniframe.host.postData(message);
    };

    external.postMessage = external.sendMessage;

    if (!window.__blazorDispatchHooked) {
        window.__blazorDispatchHooked = true;

        window.infiniframe?.host?.receiveCallback((message: string) => {
            for (const callback of callbacks) {
                try {
                    callback(message);
                } catch {
                    // Blazor callbacks are user code. Keep the host dispatch path alive.
                }
            }
        });
    }
}

/**
 * Ensures `window.external` exists, creating an empty object on it if it does not.
 *
 * @returns The {@link InfiniFrameExternal} object attached to `window.external`.
 */
function ensureWindowExternal(): InfiniFrameExternal {
    if (window.external) {
        return window.external as InfiniFrameExternal;
    }

    const external = {} as InfiniFrameExternal;
    Object.defineProperty(window, "external", {
        configurable: true,
        enumerable: true,
        value: external,
        writable: true
    });

    return external;
}
