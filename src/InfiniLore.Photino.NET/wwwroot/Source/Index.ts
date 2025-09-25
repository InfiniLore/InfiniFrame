// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

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
    interface External {
        sendMessage?(message: string): void;
    }
}



function sendMessageToHost(message: string) {
    // Try different messaging methods for Photino.NET
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage(message);
    } else if (window.external?.sendMessage) {
        window.external.sendMessage(message);
    } else {
        // Fallback for development
        console.log("Message to host failed:", message);
    }
}

const onFullscreenChange = (event: Event) => {
    if (document.fullscreenElement) {
        sendMessageToHost("fullscreen:enter");
    } else {
        sendMessageToHost("fullscreen:exit");
    }
};

document.addEventListener("fullscreenchange", onFullscreenChange);
