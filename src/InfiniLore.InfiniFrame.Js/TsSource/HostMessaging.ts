// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    IHostMessaging,
    MessageCallback,
    ReceiveFromHostMessageIds,
    SendToHostMessageId, SendToHostMessageIds
} from "./Contracts/IHostMessaging";
import {blankTargetHandler} from "./BlankTargetHandler";
import {getTitleObserver, TitleObserverTarget} from "./Observers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class HostMessaging implements IHostMessaging {
    private messageHandlers: Map<string, MessageCallback> = new Map();

    constructor() {
        this.assignWebMessageReceiver();

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            document.addEventListener("click", blankTargetHandler, {capture: true});
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
            document.addEventListener("fullscreenchange", (_: Event) => {
                if (document.fullscreenElement) this.sendMessageToHost(SendToHostMessageIds.fullscreenEnter);
                else this.sendMessageToHost(SendToHostMessageIds.fullscreenExit);
            });

            document.addEventListener("keydown", async (e: KeyboardEvent) => {
                if (e.key !== "F11") return;
                if (document.fullscreenElement) await document.exitFullscreen();
                else await document.body.requestFullscreen();
            });
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerTitleChange, _ => {
            if (TitleObserverTarget) getTitleObserver().observe(TitleObserverTarget, {childList: true});
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerWindowClose, _ => {
            window.close = () => {
                this.sendMessageToHost(SendToHostMessageIds.windowClose);
            }
        })
    }

    public sendMessageToHost(id: SendToHostMessageId | string, data?: string) {
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
        // Store the original receiveMessage if it exists (for Blazor compatibility)
        const originalReceiveMessage = window.external?.receiveMessage;

        // Handle WebView2 messages (Windows)
        if (window.chrome?.webview) {
            window.chrome.webview.addEventListener('message', (event) => {
                if (!this.isBlazorMessage(event.data)) {
                    this.handleWebMessage(event.data);
                }
            });
        }

        // Handle general Photino messages (cross-platform)
        if (typeof window !== 'undefined' && window.external) {
            window.external.receiveMessage = (message: any) => {
                // Check if it's a Blazor message and pass it to the original handler
                if (this.isBlazorMessage(message)) {
                    if (originalReceiveMessage) {
                        originalReceiveMessage(message);
                    }
                    return;
                }

                // Handle our custom messages
                this.handleWebMessage(message);
            };
        }
    }

    private isBlazorMessage(message: any): boolean {
        if (typeof message !== 'string') return true; // Assume non-string messages are Blazor

        // Check for common Blazor message patterns
        return message.startsWith('__bwv:')
            || message.startsWith('e=>{')
            || message.includes('BeginInvokeJS')
            || message.includes('AttachToDocument')
            || message.includes('RenderBatch')
            || message.includes('Blazor.');
    }

    private handleWebMessage(message: any) {
        // Ensure message is a string
        const messageStr = typeof message === 'string' ? message : String(message || '');

        if (!messageStr) {
            console.warn('Received empty or invalid message');
            return;
        }

        let messageId: string;
        let data: string | undefined;

        // Parse message - check if it contains data separated by semicolon
        if (messageStr.includes(';')) {
            const parts = messageStr.split(';', 2);
            messageId = parts[0];
            data = parts[1];
        } else {
            messageId = messageStr;
        }

        // Execute registered handler
        const handler = this.messageHandlers.get(messageId);
        if (handler) {
            handler(data);
        } else {
            console.warn('No handler registered for message ID:', messageId);
        }
    }

    public assignMessageReceivedHandler(messageId: string, callback: MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }

    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }
}

export default HostMessaging
