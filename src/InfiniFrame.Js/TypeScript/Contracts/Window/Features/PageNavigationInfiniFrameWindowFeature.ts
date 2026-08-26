// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface PageNavigationInfiniFrameWindowFeature {
    loadUri(uri: string): void;

    loadPath(path: string): void;

    tryLoadUriAsync(uri: string): Promise<boolean>;

    tryLoadPathAsync(path: string): Promise<boolean>;

    loadRawString(content: string): void;

    getCurrentUrlAsync(): Promise<string | null>;

    getCurrentUriAsync(): Promise<string | null>;
}
