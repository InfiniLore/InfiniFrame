// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    GetMessageFromHostMessageIds,
    InfiniFrameWindowFeatureDecorations as InfiniFrameWindowFeatureDecorationsContract,
    SendToHostMessageIds
} from "../../Contracts";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeatureDecorations implements InfiniFrameWindowFeatureDecorationsContract {


    setTitle(title:string) {
        window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.setTitle, title);
    }

    async getTitleAsync(): Promise<string> {
        return window.infiniframe.messaging.getMessageFromHostAsync(GetMessageFromHostMessageIds.getTitle);
    }
}