// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

import {InteropEnvelopeV1} from "./EnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const infiniFrame: string = "__infiniframe";

export const SendToHostMessageIds = {
    titleChange: `${infiniFrame}:title:change`,
    fullscreenEnter: `${infiniFrame}:fullscreen:enter`,
    fullscreenExit: `${infiniFrame}:fullscreen:exit`,
    openExternalLink: `${infiniFrame}:open:external`,
    windowClose: `${infiniFrame}:window:close`,
    ready: `${infiniFrame}:ready`,
    getMessageRequest: `${infiniFrame}:get:request`,
}

export const ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniFrame}:register:open:external`,
    registerFullscreenChange: `${infiniFrame}:register:fullscreen:change`,
    registerTitleChange: `${infiniFrame}:register:title:change`,
    registerWindowClose: `${infiniFrame}:register:window:close`,
    getMessageResponse: `${infiniFrame}:get:response`,
}

export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];
export type MessageCallback = (data?: string) => void;

export interface IInfiniFrameHostMessaging {
    sendMessageToHost(id: SendToHostMessageId | string, data?: unknown): void;
    getMessageFromHost(message: InteropEnvelopeV1 | string): Promise<string>;

    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    unregisterMessageReceivedHandler(messageId: string): void;
}
