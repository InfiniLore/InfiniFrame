// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    ReceiveFromHostMessageIds,
    SendToHostMessageIds
} from "./Contracts";
import type {
    InfiniFrameHostMessaging as InfiniFrameHostMessagingContract,
    InteropEnvelopeV1,
    MessageCallback,
    SendToHostMessageId
} from "./Contracts";
import {
    createEnvelope,
    createGetEnvelope,
    InteropGetCommand,
    parseIncomingMessage
} from "./Interop/EnvelopeProtocol/InteropEnvelopeProtocol";
import {blankTargetHandler, getTitleObserver, getTitleObserverTarget} from "./Utils";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameHostMessaging implements InfiniFrameHostMessagingContract {
    private static readonly BlazorWebViewMessagePrefix = "__bwv:";
    private messageHandlers: Map<string, MessageCallback> = new Map();
    private openExternalRegistered = false;
    private fullscreenRegistered = false;
    private titleRegistered = false;
    private windowCloseRegistered = false;
    private readyHandshakeAcknowledged = false;
    private resolveReady!: () => void;
    public readonly ready: Promise<void>;

    public get isReady(): boolean {
        return this.readyHandshakeAcknowledged;
    }
    
    constructor() {
        this.ready = new Promise<void>(resolve => {
            this.resolveReady = resolve;
        });

        this.assignWebMessageReceiver();

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

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.readyAck, _ => {
            this.markReadyHandshakeAcknowledged();
        })

        this.sendReadyHandshake();
    }

    public sendMessageToHost(id: SendToHostMessageId | string, data?: unknown) {
        const envelope = createEnvelope(id, data);

        if (window.infiniframe?.host?.postData) {
            window.infiniframe.host.postData(envelope);
        } else {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }
    }
    
    public async getMessageFromHostRawAsync(message: InteropEnvelopeV1 | string): Promise<string> {
        const host = window.infiniframe?.host;
        if (!host?.getDataAsync) throw new Error("Message to host failed. Host getDataAsync API is not initialized.");

        const envelope = typeof message === "string"
            ? createEnvelope(message, undefined, undefined, InteropGetCommand)
            : message;

        return await host.getDataAsync(envelope);
    }
    
    public async getMessageFromHostAsync(command: string, args?: any): Promise<string> {
        try {
            return window.infiniframe.messaging.getMessageFromHostRawAsync(
                createGetEnvelope(command, args)
            );
        }
        catch (e) {
            console.error("Failed to get response message from host.", e);
            return Promise.reject(e);
        }
    }

    public assignMessageReceivedHandler(messageId: string, callback: MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }

    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }

    private assignWebMessageReceiver() {
        if (window.infiniframe?.host?.receiveCallback) {
            window.infiniframe.host.receiveCallback((message: string) => {
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
        if ("error" in parsedMessage) return false;

        // Blazor WebView internal transport messages are routed by blazor.webview.js.
        // They are not InfiniFrame host-message contracts and should not emit warnings.
        if (parsedMessage.messageId.startsWith(InfiniFrameHostMessaging.BlazorWebViewMessagePrefix)) {
            return true;
        }

        // Execute registered handler
        const handler = this.messageHandlers.get(parsedMessage.messageId);
        if (!handler) {
            console.warn('No handler registered for message:', parsedMessage);
            return false;
        }

        handler(parsedMessage.payload);
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

    private sendReadyHandshake() {
        this.sendMessageToHost(SendToHostMessageIds.ready);
    }

    private markReadyHandshakeAcknowledged() {
        if (this.readyHandshakeAcknowledged) return;
        this.readyHandshakeAcknowledged = true;
        this.resolveReady();
    }
}

export default InfiniFrameHostMessaging
