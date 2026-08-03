// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {FilePickerFilter,FilePickerDialogsInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class FilePickerDialogsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("filePickerDialogs");}

    showOpenFileAsync(title = "Choose file", defaultPath: string | null = null, multiSelect = false, filters: FilePickerFilter[] | null = null) {
        return this.get<(string | null)[]>("showOpenFile", {title, defaultPath, multiSelect, filters});
    }

    showOpenFolderAsync(title = "Select folder", defaultPath: string | null = null, multiSelect = false) {
        return this.get<(string | null)[]>("showOpenFolder", {title, defaultPath, multiSelect});
    }

    showSaveFileAsync(title = "Save file", defaultPath: string | null = null, filters: FilePickerFilter[] | null = null, defaultFileName: string | null = null) {
        return this.get<string | null>("showSaveFile", {title, defaultPath, filters, defaultFileName});
    }
}
