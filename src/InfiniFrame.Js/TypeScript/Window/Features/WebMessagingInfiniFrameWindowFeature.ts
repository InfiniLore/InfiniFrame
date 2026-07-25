// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {WebMessagingInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class WebMessagingInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("webMessaging");}

    sendWebMessage(message: string) {
        return this.post("sendWebMessage", {message});
    }
}
