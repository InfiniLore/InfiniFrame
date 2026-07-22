// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DialogButtons,DialogIcon,DialogResult,InfiniFrameWindowFeatureNotifications as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeatureNotifications extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("notifications");}

    showNotification(title: string, body: string) {
        return this.post("showNotification", {title, body});
    }

    showMessageAsync(title: string, text: string | null = null, buttons: DialogButtons = "ok", icon: DialogIcon = "info") {
        return this.get<DialogResult>("showMessage", {title, text, buttons, icon});
    }
}
