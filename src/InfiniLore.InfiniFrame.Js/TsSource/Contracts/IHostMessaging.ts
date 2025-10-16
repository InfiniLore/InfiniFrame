// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const SendToHostMessageIds = {
    titleChange: "__infiniWindow:title:change",
    fullscreenEnter: "__infiniWindow:fullscreen:enter",
    fullscreenExit: "__infiniWindow:fullscreen:exit",
    openExternalLink: "__infiniWindow:open:external",
    windowClose: "__infiniWindow:window:close",
}

export const ReceiveFromHostMessageIds = {
    registerOpenExternal: "__infiniWindow:register:open:external",
    registerFullscreenChange: "__infiniWindow:register:fullscreen:change",
    registerTitleChange: "__infiniWindow:register:title:change",
    registerWindowClose: "__infiniWindow:register:window:close",
}

export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];
export type MessageCallback = (data?: string) => void;

export interface IHostMessaging {
    sendMessageToHost(id: SendToHostMessageId | string, data?: string): void;

    assignMessageReceivedHandler(messageId: string, callback: MessageCallback): void;

    unregisterMessageReceivedHandler(messageId: string): void;
}