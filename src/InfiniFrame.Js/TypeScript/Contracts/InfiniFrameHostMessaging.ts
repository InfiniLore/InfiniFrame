// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InteropEnvelopeV1} from "./EnvelopeProtocol";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const infiniframe: string = "__infiniframe";
const window: string = "window";
const features: string = "features";

const windowFeaturePrefix: string = `${infiniframe}:${window}:${features}`;

export const SendToHostMessageIds = {
    getRequest: `${infiniframe}:get`,
    fullscreenEnter: `${infiniframe}:fullscreen:enter`,
    fullscreenExit: `${infiniframe}:fullscreen:exit`,
    openExternalLink: `${infiniframe}:open:external`,
    windowClose: `${infiniframe}:window:close`,
    ready: `${infiniframe}:ready`,
    windowFeatureRequest: windowFeaturePrefix,
}

export const GetMessageFromHostMessageIds = {
    windowFeaturePrefix,
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

export interface InfiniFrameHostMessaging {
    readonly ready: Promise<void>;
    readonly isReady: boolean;

    sendMessageToHost(id: SendToHostMessageId | string, data?: unknown): void;
    getMessageFromHostRawAsync(message: InteropEnvelopeV1 | string): Promise<string>;
    getMessageFromHostAsync(message: string, args?: any): Promise<string>;

    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    unregisterMessageReceivedHandler(messageId: string): void;
}
