// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {FilePickerFilter} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureFilePickerDialogs {
    showOpenFileAsync(title?: string, defaultPath?: string | null, multiSelect?: boolean, filters?: FilePickerFilter[] | null): Promise<(string | null)[]>;
    showOpenFolderAsync(title?: string, defaultPath?: string | null, multiSelect?: boolean): Promise<(string | null)[]>;
    showSaveFileAsync(title?: string, defaultPath?: string | null, filters?: FilePickerFilter[] | null): Promise<string | null>;
}
