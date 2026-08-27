/**
 * Desktop notifications and message box dialogs feature.
 *
 * @module NotificationsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    DialogButtons,
    DialogIcon,
    DialogResult,
    NotificationsInfiniFrameWindowFeature as Contract
} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides desktop notification display and native message box dialog functionality.
 */
export class NotificationsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new notifications feature instance.
     */
    constructor() {
        super("notifications");
    }

    /**
     * Shows a desktop notification with the given title and body.
     *
     * @param title - The notification title.
     * @param body - The notification body text.
     */
    showNotification(title: string, body: string) {
        return this.post("showNotification", {title, body});
    }

    /**
     * Shows a native message box dialog with the specified configuration.
     *
     * @param title - The dialog title.
     * @param text - The message text, or `null` for no body.
     * @param buttons - The button configuration. Defaults to `"ok"`.
     * @param icon - The dialog icon. Defaults to `"info"`.
     * @returns A promise that resolves to the {@link DialogResult} indicating which button was clicked.
     */
    showMessageAsync(title: string, text: string | null = null, buttons: DialogButtons = "ok", icon: DialogIcon = "info") {
        return this.get<DialogResult>("showMessage", {title, text, buttons, icon});
    }
}
