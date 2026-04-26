// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    IInfiniFrameHostMessaging,
    MessageCallback,
    ReceiveFromHostMessageIds,
    SendToHostMessageId,
    SendToHostMessageIds
} from "./Contracts";
import {createEnvelope, parseIncomingMessage} from "./Interop/EnvelopeProtocol/InteropEnvelopeProtocol";
import {blankTargetHandler} from "./Utils/BlankTargetHandler";
import {getTitleObserver, getTitleObserverTarget} from "./Utils/Observers";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameHostMessaging implements IInfiniFrameHostMessaging {
    private static readonly BlazorWebViewMessagePrefix = "__bwv:";
    private static readonly ReadyHandshakeRetryIntervalMs = 1000;
    private static readonly MaxReadyHandshakeAttempts = 20;
    private messageHandlers: Map<string, MessageCallback> = new Map();
    private openExternalRegistered = false;
    private fullscreenRegistered = false;
    private titleRegistered = false;
    private windowCloseRegistered = false;
    private legacyInboundWarningLogged = false;
    private readyHandshakeAttempts = 0;
    private readyHandshakeAcknowledged = false;
    private readyHandshakeRetryTimer: number | null = null;
    
    constructor() {
        this.assignWebMessageReceiver();

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerOpenExternal();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerFullscreenChange();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerTitleChange, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerTitleChange();
        })

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerWindowClose, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerWindowClose();
        })

        this.sendReadyHandshakeWithRetry();
    }

    public sendMessageToHost(id: SendToHostMessageId | string, data?: unknown) {
        const envelope = createEnvelope(id, data);

        if (window.infiniframe?.host?.postMessage) {
            window.infiniframe.host.postMessage(envelope);
        } else {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }
    }

    public assignMessageReceivedHandler(messageId: string, callback: MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }

    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }

    private assignWebMessageReceiver() {
        if (window.infiniframe?.host?.receiveMessage) {
            window.infiniframe.host.receiveMessage((message: string) => {
                this.handleInteropMessage(message);
            });
        }
        else {
            console.warn("Web message receiver failed. Host bridge API is not initialized.");
            return;
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

        // Blazor WebView internal transport messages are routed by blazor.webview.js.
        // They are not InfiniFrame host-message contracts and should not emit warnings.
        if (parsedMessage.messageId.startsWith(InfiniFrameHostMessaging.BlazorWebViewMessagePrefix)) {
            return true;
        }

        if (parsedMessage.isLegacyProtocol && !this.legacyInboundWarningLogged) {
            this.legacyInboundWarningLogged = true;
            console.warn("Received legacy inbound host message format. Migrate host-to-web messages to the JSON envelope contract.");
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

    private sendReadyHandshakeWithRetry() {
        this.sendReadyHandshake();

        this.readyHandshakeRetryTimer = window.setInterval(() => {
            if (this.readyHandshakeAcknowledged || this.readyHandshakeAttempts >= InfiniFrameHostMessaging.MaxReadyHandshakeAttempts) {
                this.stopReadyHandshakeRetry();
                return;
            }

            this.sendReadyHandshake();
        }, InfiniFrameHostMessaging.ReadyHandshakeRetryIntervalMs);
    }

    private sendReadyHandshake() {
        this.readyHandshakeAttempts++;
        this.sendMessageToHost(SendToHostMessageIds.ready);
    }

    private markReadyHandshakeAcknowledged() {
        if (this.readyHandshakeAcknowledged) return;
        this.readyHandshakeAcknowledged = true;
        this.stopReadyHandshakeRetry();
    }

    private stopReadyHandshakeRetry() {
        if (this.readyHandshakeRetryTimer === null) return;
        window.clearInterval(this.readyHandshakeRetryTimer);
        this.readyHandshakeRetryTimer = null;
    }
}

export default InfiniFrameHostMessaging
