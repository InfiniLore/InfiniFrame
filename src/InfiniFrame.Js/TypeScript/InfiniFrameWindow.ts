// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameWindow, SendToHostMessageIds} from "./Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindow implements IInfiniFrameWindow {
    
    setTitle(title:string) {
        document.title = title;
        
        window.infiniFrame.hostMessaging.sendMessageToHost(SendToHostMessageIds.titleChange, title);
    } 
}