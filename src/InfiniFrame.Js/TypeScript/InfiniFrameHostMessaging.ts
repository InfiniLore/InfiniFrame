/**
 * Low-level messaging bridge between JavaScript and the native host window. Handles sending/receiving messages via the WebView2/WebKit postMessage API.
 * @module InfiniFrameHostMessaging
 */

// ---------------------------------------------------------------------------------------------------------------------
import type {
    InfiniFrameHostMessaging as InfiniFrameHostMessagingContract,
    InteropEnvelopeV1,
    MessageCallback,
    SendToHostMessageId
} from "./Contracts";
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {ReceiveFromHostMessageIds, SendToHostMessageIds} from "./Contracts";
import {
    createEnvelope,
    createGetEnvelope,
    InteropGetCommand,
    parseIncomingMessage
} from "./Interop/EnvelopeProtocol/InteropEnvelopeProtocol";
import {blankTargetHandler, getTitleObserver, getTitleObserverTarget} from "./Utils";
import {
    handleJavaScriptEvalRequest,
    handleJavaScriptEvalResponse
} from "./Window/Features/JavaScriptInfiniFrameWindowFeature";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides the messaging transport layer for communicating with the C# host.
 *
 * Wraps the WebView2/WebKit postMessage API and exposes a typed request/response
 * pattern on top of it. Incoming messages are dispatched to registered handlers
 * keyed by message ID.
 */
class InfiniFrameHostMessaging implements InfiniFrameHostMessagingContract {
    private static readonly BlazorWebViewMessagePrefix = "__bwv:";

    /** Resolves when the host acknowledges the ready handshake. */
    public readonly ready: Promise<void>;

    private messageHandlers: Map<string, MessageCallback> = new Map();
    private openExternalRegistered = false;
    private fullscreenRegistered = false;
    private titleRegistered = false;
    private windowCloseRegistered = false;
    private readyHandshakeAcknowledged = false;
    private resolveReady!: () => void;

    /**
     * Initialises the messaging layer.
     *
     * Sets up the web-message receiver, registers built-in message handlers
     * for host-initiated features (open-external, fullscreen, title, window-close),
     * and fires the ready handshake.
     */
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

        this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.webMessageAckRequest, payload => {
            if (!payload) return;
            try {
                const request = JSON.parse(payload) as { OperationId?: string; Message?: string };
                if (!request.OperationId || typeof request.Message !== "string") return;
                if (!this.handleInteropMessage(request.Message)) return;
                this.sendMessageToHost(SendToHostMessageIds.webMessageAckResponse, request.OperationId);
            } catch (error) {
                console.warn("Could not process acknowledged host message.", error);
            }
        })

        this.assignMessageReceivedHandler("__infiniframe:javascript:eval:response", payload => {
            if (!payload) return;
            try {
                handleJavaScriptEvalResponse(JSON.parse(payload));
            } catch (error) {
                console.warn("Could not process JavaScript eval response.", error);
            }
        })

        this.assignMessageReceivedHandler("__infiniframe:javascript:eval", payload => {
            if (!payload) return;
            try {
                handleJavaScriptEvalRequest(JSON.parse(payload));
            } catch (error) {
                console.warn("Could not process JavaScript eval request.", error);
            }
        })

        this.sendReadyHandshake();
    }

    /**
     * Whether the ready handshake with the host has been acknowledged.
     * @returns `true` if the host has responded with a readyAck message.
     */
    public get isReady(): boolean {
        return this.readyHandshakeAcknowledged;
    }

    /**
     * Sends a message to the host window via the postMessage bridge.
     * @param id - The message identifier (one of {@link SendToHostMessageIds} or an arbitrary string).
     * @param data - Optional payload to include with the message.
     */
    public sendMessageToHost(id: SendToHostMessageId | string, data?: unknown) {
        const envelope = createEnvelope(id, data);

        if (window.infiniframe?.host?.postData) {
            window.infiniframe.host.postData(envelope);
        } else {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }
    }

    /**
     * Retrieves a raw response string from the host for a given interop envelope.
     * @param message - An {@link InteropEnvelopeV1} envelope or a message-id string.
     * @returns A promise that resolves with the raw response string from the host.
     * @throws If the host `getDataAsync` API is not initialised.
     */
    public async getMessageFromHostRawAsync(message: InteropEnvelopeV1 | string): Promise<string> {
        const host = window.infiniframe?.host;
        if (!host?.getDataAsync) throw new Error("Message to host failed. Host getDataAsync API is not initialized.");

        const envelope = typeof message === "string"
            ? createEnvelope(message, undefined, undefined, InteropGetCommand)
            : message;

        return await host.getDataAsync(envelope);
    }

    /**
     * Convenience wrapper that builds a GET envelope from a command name and
     * optional arguments, then retrieves the response from the host.
     * @param command - The interop command identifier.
     * @param args - Optional arguments serialised into the request envelope.
     * @returns A promise that resolves with the response string from the host.
     */
    public async getMessageFromHostAsync(command: string, args?: any): Promise<string> {
        try {
            return await window.infiniframe.messaging.getMessageFromHostRawAsync(
                createGetEnvelope(command, args)
            );
        } catch (e) {
            console.error("Failed to get response message from host.", e);
            throw e;
        }
    }

    /**
     * Registers a callback that will be invoked when a message with the given
     * identifier arrives from the host.
     * @param messageId - The message identifier to listen for.
     * @param callback - Handler that receives the optional message payload.
     */
    public assignMessageReceivedHandler(messageId: string, callback: MessageCallback) {
        this.messageHandlers.set(messageId, callback);
    }

    /**
     * Removes a previously registered message handler.
     * @param messageId - The message identifier whose handler should be removed.
     */
    public unregisterMessageReceivedHandler(messageId: string) {
        this.messageHandlers.delete(messageId);
    }

    private assignWebMessageReceiver() {
        if (window.infiniframe?.host?.receiveCallback) {
            window.infiniframe.host.receiveCallback((message: string) => {
                this.handleInteropMessage(message);
            });
        } else {
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
            try {
                if (document.fullscreenElement) await document.exitFullscreen();
                else await document.body.requestFullscreen();
            } catch (error) {
                console.warn("Fullscreen toggle failed.", error);
            }
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
