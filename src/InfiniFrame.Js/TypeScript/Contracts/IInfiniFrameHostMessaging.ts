// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InteropEnvelopeV1} from "./EnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const infiniframe: string = "__infiniframe";

export const SendToHostMessageIds = {
    getRequest: `${infiniframe}:get`,
    titleChange: `${infiniframe}:title:change`,
    fullscreenEnter: `${infiniframe}:fullscreen:enter`,
    fullscreenExit: `${infiniframe}:fullscreen:exit`,
    openExternalLink: `${infiniframe}:open:external`,
    windowClose: `${infiniframe}:window:close`,
    ready: `${infiniframe}:ready`,
}

export const ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniframe}:register:open:external`,
    registerFullscreenChange: `${infiniframe}:register:fullscreen:change`,
    registerTitleChange: `${infiniframe}:register:title:change`,
    registerWindowClose: `${infiniframe}:register:window:close`,
    readyAck: `${infiniframe}:ready:ack`,
    getMessageResponse: `${infiniframe}:get:response`,
}

export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];
export type MessageCallback = (data?: string) => void;

export interface IInfiniFrameHostMessaging {
    readonly ready: Promise<void>;
    readonly isReady: boolean;

    sendMessageToHost(id: SendToHostMessageId | string, data?: unknown): void;
    getMessageFromHostAsync(message: InteropEnvelopeV1 | string): Promise<string>;

    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    unregisterMessageReceivedHandler(messageId: string): void;
}
