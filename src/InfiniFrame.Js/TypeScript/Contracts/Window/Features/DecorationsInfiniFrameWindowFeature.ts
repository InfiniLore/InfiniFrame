// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface DecorationsInfiniFrameWindowFeature {
    isChromelessAsync(): Promise<boolean>;

    isTransparentAsync(): Promise<boolean>;

    backgroundColorAsync(): Promise<string | null>;

    getTitleAsync(): Promise<string | null>;

    getIconFilePathAsync(): Promise<string | null>;

    getLimitLinuxWindowTitleLengthAsync(): Promise<boolean>;

    setTransparent(enabled?: boolean): void;

    setBackgroundColor(color: string | null): void;

    setTitle(title: string | null): void;

    setIconFile(iconFilePath: string): void;

    setLimitLinuxWindowTitleLength(enabled?: boolean): void;
}
