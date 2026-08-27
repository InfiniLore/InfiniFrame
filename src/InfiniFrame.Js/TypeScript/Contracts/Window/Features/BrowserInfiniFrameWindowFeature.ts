/**
 * Browser feature contract. Defines the JS API for querying and controlling browser-level settings
 * such as context menu, media autoplay, user agent, web security, and clipboard access.
 * @module BrowserInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Browser-level feature API for the InfiniFrame window.
 * Provides methods to query and modify browser behavior settings.
 */
export interface BrowserInfiniFrameWindowFeature {
    /**
     * Checks whether the browser context menu is enabled.
     * @returns A promise resolving to true if the context menu is enabled.
     */
    isContextMenuEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether media autoplay is enabled.
     * @returns A promise resolving to true if media autoplay is allowed.
     */
    isMediaAutoplayEnabledAsync(): Promise<boolean>;

    /**
     * Gets the custom user agent string set on the browser control.
     * @returns A promise resolving to the user agent string, or null if using the default.
     */
    getUserAgentAsync(): Promise<string | null>;

    /**
     * Checks whether the File System Access API is enabled.
     * @returns A promise resolving to true if file system access is allowed.
     */
    isFileSystemAccessEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether web security (CORS, same-origin policy) is enabled.
     * @returns A promise resolving to true if web security is active.
     */
    isWebSecurityEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether JavaScript clipboard access (e.g. navigator.clipboard) is enabled.
     * @returns A promise resolving to true if clipboard access is allowed.
     */
    isJavascriptClipboardAccessEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether media stream access (camera/microphone) is enabled.
     * @returns A promise resolving to true if media streams are allowed.
     */
    isMediaStreamEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether the browser ignores certificate errors.
     * @returns A promise resolving to true if certificate errors are ignored.
     */
    isIgnoreCertificateErrorsEnabledAsync(): Promise<boolean>;

    /**
     * Checks whether the browser grants all permission requests automatically.
     * @returns A promise resolving to true if permissions are auto-granted.
     */
    getGrantBrowserPermissionsAsync(): Promise<boolean>;

    /**
     * Checks whether smooth scrolling is enabled.
     * @returns A promise resolving to true if smooth scrolling is active.
     */
    isSmoothScrollingEnabledAsync(): Promise<boolean>;

    /**
     * Gets the initialization parameters for the browser control.
     * @returns A promise resolving to the parameters string, or null if none are set.
     */
    getBrowserControlInitParametersAsync(): Promise<string | null>;

    /**
     * Enables or disables the browser context menu.
     * @param enabled - true to enable, false to disable. Defaults to true.
     */
    enableContextMenu(enabled?: boolean): void;

    /**
     * Enables or disables media autoplay.
     * @param enabled - true to allow autoplay, false to block it. Defaults to true.
     */
    enableMediaAutoplay(enabled?: boolean): void;

    /**
     * Sets a custom user agent string on the browser control.
     * @param userAgent - The user agent string, or null to reset to default.
     */
    setUserAgent(userAgent: string | null): void;

    /**
     * Sets the WebView2 installation path on Windows.
     * @param path - File system path to the WebView2 runtime.
     */
    win32SetWebView2Path(path: string): void;

    /**
     * Clears all browser auto-fill data (form data, passwords, etc.).
     */
    clearBrowserAutoFill(): void;
}
