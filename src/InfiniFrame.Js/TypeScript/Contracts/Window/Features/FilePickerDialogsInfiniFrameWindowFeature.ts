/**
 * File picker dialogs feature contract. Defines the JS API for native file open,
 * folder open, and save file dialogs.
 * @module FilePickerDialogsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {FilePickerFilter} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * File picker dialogs feature API for the InfiniFrame window.
 * Provides methods to show native file and folder selection dialogs.
 */
export interface FilePickerDialogsInfiniFrameWindowFeature {
    /**
     * Shows a native open file dialog.
     * @param title - Dialog title text.
     * @param defaultPath - Initial directory path, or null for the default.
     * @param multiSelect - Whether multiple files can be selected.
     * @param filters - Array of file type filters, or null for all files.
     * @returns A promise resolving to an array of selected file paths (empty if cancelled).
     */
    showOpenFileAsync(title?: string, defaultPath?: string | null, multiSelect?: boolean, filters?: FilePickerFilter[] | null): Promise<(string | null)[]>;

    /**
     * Shows a native open folder dialog.
     * @param title - Dialog title text.
     * @param defaultPath - Initial directory path, or null for the default.
     * @param multiSelect - Whether multiple folders can be selected.
     * @returns A promise resolving to an array of selected folder paths (empty if cancelled).
     */
    showOpenFolderAsync(title?: string, defaultPath?: string | null, multiSelect?: boolean): Promise<(string | null)[]>;

    /**
     * Shows a native save file dialog.
     * @param title - Dialog title text.
     * @param defaultPath - Initial directory path, or null for the default.
     * @param filters - Array of file type filters, or null for all files.
     * @param defaultFileName - Pre-filled file name, or null for none.
     * @returns A promise resolving to the selected file path, or null if cancelled.
     */
    showSaveFileAsync(title?: string, defaultPath?: string | null, filters?: FilePickerFilter[] | null, defaultFileName?: string | null): Promise<string | null>;
}
