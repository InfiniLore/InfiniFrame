/**
 * Web messaging feature. Sends structured messages from JavaScript to the C# host.
 *
 * @module WebMessagingInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {WebMessagingInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides a fire-and-forget mechanism for sending web messages to the C# host.
 */
export class WebMessagingInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new web messaging feature instance.
     */
    constructor() {
        super("webMessaging");
    }

    /**
     * Sends a web message to the C# host.
     *
     * @param message - The message string to send.
     */
    sendWebMessage(message: string) {
        return this.post("sendWebMessage", {message});
    }
}
