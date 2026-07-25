// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DialogButtons, DialogIcon, DialogResult} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface NotificationsInfiniFrameWindowFeature {
    showNotification(title: string, body: string): void;
    showMessageAsync(title: string, text?: string | null, buttons?: DialogButtons, icon?: DialogIcon): Promise<DialogResult>;
}
