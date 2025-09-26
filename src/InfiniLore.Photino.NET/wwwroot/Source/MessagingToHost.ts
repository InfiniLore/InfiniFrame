// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function sendMessageToHost(message: string) {
    // TODO - determine messaging methods for Photino.NET for all platforms
    
    // Try different messaging methods for Photino.NET
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage(message);
    } else if (window.external?.sendMessage) {
        window.external.sendMessage(message);
    } else {
        // Fallback for development
        console.warn("Message to host failed:", message);
    }
}