/**
 * File picker dialogs feature. Shows native open-file, open-folder, and save-file dialogs.
 *
 * @module FilePickerDialogsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {FilePickerDialogsInfiniFrameWindowFeature as Contract, FilePickerFilter} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides native file and folder picker dialogs for open, folder-select, and save operations.
 */
export class FilePickerDialogsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new file picker dialogs feature instance.
     */
    constructor() {
        super("filePickerDialogs");
    }

    /**
     * Shows a native open-file dialog.
     *
     * @param title - The dialog title. Defaults to `"Choose file"`.
     * @param defaultPath - The initial directory path, or `null` for the system default.
     * @param multiSelect - Whether multiple files can be selected. Defaults to `false`.
     * @param filters - Optional array of file type filters.
     * @returns A promise that resolves to an array of selected file paths, or an array containing `null` if cancelled.
     */
    showOpenFileAsync(title = "Choose file", defaultPath: string | null = null, multiSelect = false, filters: FilePickerFilter[] | null = null) {
        return this.get<(string | null)[]>("showOpenFile", {title, defaultPath, multiSelect, filters});
    }

    /**
     * Shows a native open-folder dialog.
     *
     * @param title - The dialog title. Defaults to `"Select folder"`.
     * @param defaultPath - The initial directory path, or `null` for the system default.
     * @param multiSelect - Whether multiple folders can be selected. Defaults to `false`.
     * @returns A promise that resolves to an array of selected folder paths, or an array containing `null` if cancelled.
     */
    showOpenFolderAsync(title = "Select folder", defaultPath: string | null = null, multiSelect = false) {
        return this.get<(string | null)[]>("showOpenFolder", {title, defaultPath, multiSelect});
    }

    /**
     * Shows a native save-file dialog.
     *
     * @param title - The dialog title. Defaults to `"Save file"`.
     * @param defaultPath - The initial file path, or `null` for the system default.
     * @param filters - Optional array of file type filters.
     * @param defaultFileName - Optional default file name to suggest.
     * @returns A promise that resolves to the selected file path, or `null` if cancelled.
     */
    showSaveFileAsync(title = "Save file", defaultPath: string | null = null, filters: FilePickerFilter[] | null = null, defaultFileName: string | null = null) {
        return this.get<string | null>("showSaveFile", {title, defaultPath, filters, defaultFileName});
    }
}
