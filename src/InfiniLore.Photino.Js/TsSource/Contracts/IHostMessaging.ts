// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export const HostMessageIds = {
    titleChange: "title:change",
    fullscreenEnter: "fullscreen:enter",
    fullscreenExit: "fullscreen:exit",
    openExternalLink: "open:external",
}

export type HostMessageId = typeof HostMessageIds[keyof typeof HostMessageIds];

export interface IHostMessaging {
    sendMessageToHost(id: HostMessageId, data?: string): void;
}