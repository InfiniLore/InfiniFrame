// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindow as InfiniFrameWindowContract} from "./Contracts";
import {SendToHostMessageIds} from "./Contracts";
import {createGetEnvelope} from "./Interop/EnvelopeProtocol/InteropEnvelopeProtocol";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindow implements InfiniFrameWindowContract {
    
    private getMessageFromHostAsync(command: string, args?: any): Promise<string> {
        try {
            return window.infiniframe.messaging.getMessageFromHostAsync(
                createGetEnvelope(command, args)
            );
        }
        catch (e) {
            console.error("Failed to get response message from host.", e);
            return Promise.reject(e);
        }
    }
    
    setTitle(title:string) {
        window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.titleChange, title);
    }

    async getTitleAsync(): Promise<string> {
        return this.getMessageFromHostAsync("title")
    }
}
