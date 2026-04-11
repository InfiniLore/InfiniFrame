// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    IHostMessaging,
    MessageCallback,
    ReceiveFromHostMessageIds,
    SendToHostMessageId,
    SendToHostMessageIds
} from "./Contracts/IHostMessaging";
import {createEnvelopeMessage, parseIncomingMessage} from "./Interop/InteropEnvelopeProtocol";
import {blankTargetHandler} from "./BlankTargetHandler";
import {getTitleObserver, getTitleObserverTarget} from "./Observers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class HostMessaging implements IHostMessaging {
    private messageHandlers: Map<string, MessageCallback> = new Map();
    private openExternalRegistered = false;
    private fullscreenRegistered = false;
    private titleRegistered = false;
    private windowCloseRegistered = false;

    constructor() {
        this.assignWebMessageReceiver();
        this.sendMessageToHost(SendToHostMessageIds.ready);

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            this.registerOpenExternal();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
            this.registerFullscreenChange();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerTitleChange, _ => {
            this.registerTitleChange();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerWindowClose, _ => {
            this.registerWindowClose();
        })
    }

    public sendMessageToHost(id: SendToHostMessageId | string, data?: unknown) {
        const message = createEnvelopeMessage(id, data);

        // TODO - determine messaging methods for InfiniFrame.NET for all platforms
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
                this.handleInteropMessage(event.data);
            });
        }

        // Handle general InfiniFrame messages (cross-platform)
        if (typeof window !== 'undefined' && window.external) {
            window.external.receiveMessage = (message: any) => {
                if (this.handleInteropMessage(message)) {
                    return;
                }

                originalReceiveMessage?.(message);
            };
        }
    }

    private handleInteropMessage(message: any): boolean {
        if (typeof message !== 'string') return false;
        if (!message) return false;
        // Route only messages that match the explicit interop envelope contract.
        const parsedMessage = parseIncomingMessage(message);
        if ("error" in parsedMessage) {
            return false;
        }

        // Execute registered handler
        const handler = this.messageHandlers.get(parsedMessage.messageId);
        if (handler) {
            handler(parsedMessage.payload);
        } else {
            console.warn('No handler registered for message ID:', parsedMessage.messageId);
        }

        return true;
    }

    public assignMessageReceivedHandler(messageId: string, callback: MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }

    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }

    private registerOpenExternal() {
        if (this.openExternalRegistered) return;
        this.openExternalRegistered = true;
        document.addEventListener("click", blankTargetHandler, {capture: true});
    }

    private registerFullscreenChange() {
        if (this.fullscreenRegistered) return;
        this.fullscreenRegistered = true;
        document.addEventListener("fullscreenchange", (_: Event) => {
            if (document.fullscreenElement) this.sendMessageToHost(SendToHostMessageIds.fullscreenEnter);
            else this.sendMessageToHost(SendToHostMessageIds.fullscreenExit);
        });

        document.addEventListener("keydown", async (e: KeyboardEvent) => {
            if (e.key !== "F11") return;
            if (document.fullscreenElement) await document.exitFullscreen();
            else await document.body.requestFullscreen();
        });
    }

    private registerTitleChange() {
        if (this.titleRegistered) return;
        this.titleRegistered = true;
        const titleTarget = getTitleObserverTarget();
        if (titleTarget) {
            getTitleObserver().observe(titleTarget, {childList: true});
            return;
        }

        const headTarget = document.head || document.documentElement;
        if (!headTarget) return;

        const headObserver = new MutationObserver(() => {
            const target = getTitleObserverTarget();
            if (!target) return;
            headObserver.disconnect();
            getTitleObserver().observe(target, {childList: true});
        });
        headObserver.observe(headTarget, {childList: true, subtree: true});
    }

    private registerWindowClose() {
        if (this.windowCloseRegistered) return;
        this.windowCloseRegistered = true;
        window.close = () => {
            this.sendMessageToHost(SendToHostMessageIds.windowClose);
        };
    }
}

export default HostMessaging
