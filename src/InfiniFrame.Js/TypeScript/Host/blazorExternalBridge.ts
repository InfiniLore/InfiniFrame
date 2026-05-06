import { type Platform, nativePost } from './platform.js';

type BlazorCallback = (message: string) => void;

declare global {
    interface Window {
        __blazor_callbacks: BlazorCallback[];
        __blazor_dispatch_hooked?: boolean;
        external: {
            receiveMessage?: (cb: BlazorCallback) => void;
            receiveCallback?: (cb: BlazorCallback) => void;
            sendMessage?: (message: string) => void;
            postMessage?: (message: string) => void;
        };
    }
}

export function initWindowExternalBridge(platform: Platform): void {
    window.external = window.external || {};
    window.__blazor_callbacks = window.__blazor_callbacks || [];

    window.external.receiveMessage = (callback: BlazorCallback): void => {
        window.__blazor_callbacks.push(callback);
    };

    window.external.receiveCallback = window.external.receiveMessage;

    window.external.sendMessage = (message: string): void => {
        nativePost(platform, message);
    };

    window.external.postMessage = window.external.sendMessage;

    if (!window.__blazor_dispatch_hooked) {
        window.__blazor_dispatch_hooked = true;

        window.__infiniframe.onReceiveMessageCallbacks.push((message: string) => {
            for (const cb of window.__blazor_callbacks) {
                try {
                    cb(message);
                } catch {
                    // ignore
                }
            }
        });
    }
}
