// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const SendToHostMessageIds = {
    titleChange: "title:change",
    fullscreenEnter: "fullscreen:enter",
    fullscreenExit: "fullscreen:exit",
    openExternalLink: "open:external",
}

export type SendToHostMessageId = typeof SendToHostMessageIds[keyof typeof SendToHostMessageIds];
export type MessageCallback = (data?: string) => void;

export interface IHostMessaging {
    sendMessageToHost(id: SendToHostMessageId, data?: string): void;
    assignMessageReceivedHandler(messageId:string, callback:MessageCallback): void;
    unregisterMessageReceivedHandler(messageId: string) : void;
}