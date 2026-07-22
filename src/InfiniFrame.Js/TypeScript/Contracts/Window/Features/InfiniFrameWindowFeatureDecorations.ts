// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureDecorations {
    SetTransparent(enabled : boolean) : void;
    IsTransparentAsync(): Promise<boolean>;

    SetTitle(title: string): void;
    getTitleAsync(): Promise<string>;

    SetIconFile(iconFilePath:string): void;
    GetIconFileAsync(): Promise<string>;

    SetLimitLinuxWindowTitleLength(enabled : boolean): void;
    GetLimitLinuxWindowTitleLengthAsync(): Promise<boolean>;
}
