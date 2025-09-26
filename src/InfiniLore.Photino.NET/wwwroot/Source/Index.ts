// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {sendMessageToHost} from "./MessagingToHost";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {};

declare global {
    // noinspection JSUnusedGlobalSymbols
    interface Window {
        chrome?: {
            webview?: {
                postMessage(message: string): void;
            };
        };
    }
    // noinspection JSUnusedGlobalSymbols
    interface External {
        sendMessage?(message: string): void;
    }
}

const onFullscreenChange = (_: Event) => {
    if (document.fullscreenElement) {
        sendMessageToHost("fullscreen:enter");
    } else {
        sendMessageToHost("fullscreen:exit");
    }
};

document.addEventListener("fullscreenchange", onFullscreenChange);
