/**
 * State feature contract. Defines the JS API for window state management including
 * fullscreen, maximize, minimize, topmost, focus, and zoom controls.
 * @module StateInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Rectangle} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Window state feature API for the InfiniFrame window.
 * Provides methods to query and control the window's visual and interactive state.
 */
export interface StateInfiniFrameWindowFeature {
    /**
     * Checks whether the window is in fullscreen mode.
     * @returns A promise resolving to true if the window is fullscreen.
     */
    isFullScreenAsync(): Promise<boolean>;

    /**
     * Checks whether the window is maximized.
     * @returns A promise resolving to true if the window is maximized.
     */
    isMaximizedAsync(): Promise<boolean>;

    /**
     * Checks whether the window is minimized.
     * @returns A promise resolving to true if the window is minimized.
     */
    isMinimizedAsync(): Promise<boolean>;

    /**
     * Checks whether the window is set to always-on-top (topmost).
     * @returns A promise resolving to true if the window is topmost.
     */
    isTopMostAsync(): Promise<boolean>;

    /**
     * Checks whether the window currently has focus.
     * @returns A promise resolving to true if the window is focused.
     */
    isFocusedAsync(): Promise<boolean>;

    /**
     * Gets the current zoom factor of the web content.
     * @returns A promise resolving to the zoom factor (e.g. 1.0 for 100%).
     */
    getZoomFactorAsync(): Promise<number>;

    /**
     * Checks whether zoom functionality is enabled on the window.
     * @returns A promise resolving to true if zoom is enabled.
     */
    isZoomEnabledAsync(): Promise<boolean>;

    /**
     * Gets the cached window bounds from before entering fullscreen.
     * @returns A promise resolving to the saved rectangle.
     */
    getCachedPreFullScreenBoundsAsync(): Promise<Rectangle>;

    /**
     * Gets the cached window bounds from before maximizing.
     * @returns A promise resolving to the saved rectangle.
     */
    getCachedPreMaximizedBoundsAsync(): Promise<Rectangle>;

    /**
     * Sets the cached window bounds to restore after exiting fullscreen.
     * @param bounds - The rectangle to cache.
     */
    setCachedPreFullScreenBounds(bounds: Rectangle): void;

    /**
     * Sets the cached window bounds to restore after un-maximizing.
     * @param bounds - The rectangle to cache.
     */
    setCachedPreMaximizedBounds(bounds: Rectangle): void;

    /**
     * Maximizes or restores the window.
     * @param maximized - true to maximize, false to restore. Defaults to true.
     */
    setMaximized(maximized?: boolean): void;

    /**
     * Toggles the window between maximized and restored state.
     */
    toggleMaximized(): void;

    /**
     * Minimizes or restores the window.
     * @param minimized - true to minimize, false to restore. Defaults to true.
     */
    setMinimized(minimized?: boolean): void;

    /**
     * Enters or exits fullscreen mode.
     * @param fullScreen - true to enter fullscreen, false to exit. Defaults to true.
     */
    setFullScreen(fullScreen?: boolean): void;

    /**
     * Brings the window to the foreground and gives it focus.
     */
    setFocused(): void;

    /**
     * Sets the zoom factor for the web content.
     * @param zoom - The zoom factor (e.g. 1.0 for 100%, 1.5 for 150%).
     */
    setZoomFactor(zoom: number): void;

    /**
     * Enables or disables the zoom functionality.
     * @param enabled - true to enable zoom, false to disable. Defaults to true.
     */
    enableZoom(enabled?: boolean): void;

    /**
     * Sets or clears the always-on-top (topmost) flag.
     * @param topMost - true to make the window topmost, false to remove the flag. Defaults to true.
     */
    setTopMost(topMost?: boolean): void;
}
