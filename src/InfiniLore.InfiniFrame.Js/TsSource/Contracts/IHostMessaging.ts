// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
const infiniFrame : string = "__infiniframe";

export const SendToHostMessageIds = {
    titleChange: `${infiniFrame}:title:change`,
    fullscreenEnter: `${infiniFrame}:fullscreen:enter`,
    fullscreenExit: `${infiniFrame}:fullscreen:exit`,
    openExternalLink: `${infiniFrame}:open:external`,
    windowClose: `${infiniFrame}:window:close`,
}

export const ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniFrame}:register:open:external`,
    registerFullscreenChange: `${infiniFrame}:register:fullscreen:change`,
    registerTitleChange: `${infiniFrame}:register:title:change`,
    registerWindowClose: `${infiniFrame}:register:window:close`,
}

export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];
export type MessageCallback = (data?: string) => void;

export interface IHostMessaging {
    sendMessageToHost(id: SendToHostMessageId | string, data?: string): void;

    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    unregisterMessageReceivedHandler(messageId: string): void;
}