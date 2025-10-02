// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IHostMessaging, MessageCallback, SendToHostMessageId} from "./Contracts/IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class HostMessaging implements IHostMessaging {
    private messageHandlers: Map<string, MessageCallback> = new Map();
    
    constructor() {
        this.assignWebMessageReceiver();
    }
        
    public sendMessageToHost(id: SendToHostMessageId, data?: string) {
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

    private assignWebMessageReceiver() {
        // Handle WebView2 messages (Windows)
        if (window.chrome?.webview) {
            window.chrome.webview.addEventListener('message', (event) => {
                this.handleWebMessage(event.data);
            });
        }

        // Handle general Photino messages (cross-platform)
        if (typeof window !== 'undefined' && window.external) {
            window.external.receiveMessage = (message: string) => {
                this.handleWebMessage(message);
            };
        }
    }

    private handleWebMessage(message: string) {
        let messageId: string;
        let data: string | undefined;

        // Parse message - check if it contains data separated by semicolon
        if (message.includes(';')) {
            const parts = message.split(';', 2);
            messageId = parts[0];
            data = parts[1];
        } else {
            messageId = message;
        }

        // Execute registered handler
        const handler = this.messageHandlers.get(messageId);
        if (handler) {
            handler(data);
        } else {
            console.warn('No handler registered for message ID:', messageId);
        }
    }
    
    public assignMessageReceivedHandler(messageId:string, callback:MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }
    
    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }
}
