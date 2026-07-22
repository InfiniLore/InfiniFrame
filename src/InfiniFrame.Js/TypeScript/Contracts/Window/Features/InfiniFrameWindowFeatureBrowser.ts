// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureBrowser {
    isContextMenuEnabledAsync(): Promise<boolean>;
    isMediaAutoplayEnabledAsync(): Promise<boolean>;
    getUserAgentAsync(): Promise<string | null>;
    isFileSystemAccessEnabledAsync(): Promise<boolean>;
    isWebSecurityEnabledAsync(): Promise<boolean>;
    isJavascriptClipboardAccessEnabledAsync(): Promise<boolean>;
    isMediaStreamEnabledAsync(): Promise<boolean>;
    isIgnoreCertificateErrorsEnabledAsync(): Promise<boolean>;
    getGrantBrowserPermissionsAsync(): Promise<boolean>;
    isSmoothScrollingEnabledAsync(): Promise<boolean>;
    getBrowserControlInitParametersAsync(): Promise<string | null>;
    enableContextMenu(enabled?: boolean): void;
    enableMediaAutoplay(enabled?: boolean): void;
    setUserAgent(userAgent: string | null): void;
    win32SetWebView2Path(path: string): void;
    clearBrowserAutoFill(): void;
}
