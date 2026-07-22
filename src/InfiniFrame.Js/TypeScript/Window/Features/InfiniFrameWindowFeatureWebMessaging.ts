// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindowFeatureWebMessaging as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeatureWebMessaging extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("webMessaging");}

    sendWebMessage(message: string) {
        return this.post("sendWebMessage", {message});
    }
}
