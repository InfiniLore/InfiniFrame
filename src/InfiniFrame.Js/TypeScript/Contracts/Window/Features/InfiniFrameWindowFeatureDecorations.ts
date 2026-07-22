// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureDecorations {
    isChromelessAsync(): Promise<boolean>;
    isTransparentAsync(): Promise<boolean>;
    getTitleAsync(): Promise<string | null>;
    getIconFilePathAsync(): Promise<string | null>;
    getLimitLinuxWindowTitleLengthAsync(): Promise<boolean>;
    setTransparent(enabled?: boolean): void;
    setTitle(title: string | null): void;
    setIconFile(iconFilePath: string): void;
    setLimitLinuxWindowTitleLength(enabled?: boolean): void;
}
