// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IHostMessaging, SendToHostMessageId} from "./IHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface IInfiniWindow {
    HostMessaging : IHostMessaging;

    sendMessageToHost(id: SendToHostMessageId, data?: string): void;
    
    setPointerCapture(element: Element, pointerId: number): void;
    releasePointerCapture(element: Element, pointerId: number): void;
}
