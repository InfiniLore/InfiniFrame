// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameWindow, SendToHostMessageIds} from "./Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindow implements IInfiniFrameWindow {
    
    setTitle(title:string) {
        window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.titleChange, title);
    }
}