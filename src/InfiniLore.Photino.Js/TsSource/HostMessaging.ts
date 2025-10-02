// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {HostMessageId, IHostMessaging} from "./Contracts/IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class HostMessaging implements IHostMessaging {
        
    sendMessageToHost(id: HostMessageId, data?: string) {
        const message = data ? `${id};${data}` : id;

        // TODO - determine messaging methods for Photino.NET for all platforms
        if (window.chrome?.webview) {
            window.chrome.webview.postMessage(message);
        } else if (window.external?.sendMessage) {
            window.external.sendMessage(message);
        } else {
            console.warn("Message to host failed:", message);
        }
    }    
}
