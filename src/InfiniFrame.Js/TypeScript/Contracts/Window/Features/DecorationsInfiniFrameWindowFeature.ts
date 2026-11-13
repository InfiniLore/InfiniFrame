/**
 * Decorations feature contract. Defines the JS API for window chrome, title, icon,
 * background color, and transparency settings.
 * @module DecorationsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Window decorations feature API for the InfiniFrame window.
 * Provides methods to query and modify window chrome and visual properties.
 */
export interface DecorationsInfiniFrameWindowFeature {
    /**
     * Checks whether the window is chromeless (no native title bar or borders).
     * @returns A promise resolving to true if the window is chromeless.
     */
    isChromelessAsync(): Promise<boolean>;

    /**
     * Checks whether the window has transparency enabled.
     * @returns A promise resolving to true if the window background is transparent.
     */
    isTransparentAsync(): Promise<boolean>;

    /**
     * Gets the background color of the window.
     * @returns A promise resolving to the CSS color string, or null if transparent.
     */
    backgroundColorAsync(): Promise<string | null>;

    /**
     * Gets the window title.
     * @returns A promise resolving to the title string, or null if untitled.
     */
    getTitleAsync(): Promise<string | null>;

    /**
     * Gets the file path of the window icon.
     * @returns A promise resolving to the icon file path, or null if no icon is set.
     */
    getIconFilePathAsync(): Promise<string | null>;

    /**
     * Checks whether the window title length is limited on Linux.
     * @returns A promise resolving to true if the title length is constrained.
     */
    getLimitLinuxWindowTitleLengthAsync(): Promise<boolean>;

    /**
     * Enables or disables window transparency.
     * @param enabled - true to enable transparency, false to disable. Defaults to true.
     */
    setTransparent(enabled?: boolean): void;

    /**
     * Sets the window background color.
     * @param color - CSS color string, or null to make the background transparent.
     */
    setBackgroundColor(color: string | null): void;

    /**
     * Sets the window title.
     * @param title - Title string, or null to remove the title.
     */
    setTitle(title: string | null): void;

    /**
     * Sets the window icon from a file path.
     * @param iconFilePath - Absolute path to the icon image file.
     */
    setIconFile(iconFilePath: string): void;

    /**
     * Enables or disables the Linux window title length limit.
     * @param enabled - true to limit the title length, false to remove the limit. Defaults to true.
     */
    setLimitLinuxWindowTitleLength(enabled?: boolean): void;
}
