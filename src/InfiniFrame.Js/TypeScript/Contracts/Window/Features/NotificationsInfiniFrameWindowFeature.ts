/**
 * Notifications feature contract. Defines the JS API for toast notifications and message dialogs.
 * @module NotificationsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DialogButtons, DialogIcon, DialogResult} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Notifications and dialogs feature API for the InfiniFrame window.
 * Provides methods to display toast notifications and modal message dialogs.
 */
export interface NotificationsInfiniFrameWindowFeature {
    /**
     * Shows a toast notification.
     * @param title - Notification title text.
     * @param body - Notification body text.
     */
    showNotification(title: string, body: string): void;

    /**
     * Shows a modal message dialog with configurable buttons and icon.
     * @param title - Dialog title text.
     * @param text - Dialog body text, or null for no body.
     * @param buttons - Button combination to display.
     * @param icon - Icon to display in the dialog.
     * @returns A promise resolving to the button clicked by the user.
     */
    showMessageAsync(title: string, text?: string | null, buttons?: DialogButtons, icon?: DialogIcon): Promise<DialogResult>;
}
