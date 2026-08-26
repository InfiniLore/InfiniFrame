/**
 * Window state feature. Manages maximized, minimized, fullscreen, topmost, focused,
 * and zoom state.
 *
 * @module StateInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Rectangle, StateInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to and control over window state including maximized, minimized,
 * fullscreen, topmost, focused, and zoom properties.
 */
export class StateInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new state feature instance.
     */
    constructor() {
        super("state");
    }

    /**
     * Checks whether the window is in fullscreen mode.
     *
     * @returns A promise that resolves to `true` if the window is fullscreen.
     */
    isFullScreenAsync() {
        return this.get<boolean>("isFullScreen");
    }

    /**
     * Checks whether the window is maximized.
     *
     * @returns A promise that resolves to `true` if the window is maximized.
     */
    isMaximizedAsync() {
        return this.get<boolean>("isMaximized");
    }

    /**
     * Checks whether the window is minimized.
     *
     * @returns A promise that resolves to `true` if the window is minimized.
     */
    isMinimizedAsync() {
        return this.get<boolean>("isMinimized");
    }

    /**
     * Checks whether the window is set to topmost (always on top).
     *
     * @returns A promise that resolves to `true` if the window is topmost.
     */
    isTopMostAsync() {
        return this.get<boolean>("isTopMost");
    }

    /**
     * Checks whether the window currently has focus.
     *
     * @returns A promise that resolves to `true` if the window is focused.
     */
    isFocusedAsync() {
        return this.get<boolean>("isFocused");
    }

    /**
     * Retrieves the current zoom factor of the window.
     *
     * @returns A promise that resolves to the zoom factor (e.g. `1.0` for 100%).
     */
    getZoomFactorAsync() {
        return this.get<number>("zoomFactor");
    }

    /**
     * Checks whether zoom is enabled for the window.
     *
     * @returns A promise that resolves to `true` if zoom is enabled.
     */
    isZoomEnabledAsync() {
        return this.get<boolean>("isZoomEnabled");
    }

    /**
     * Retrieves the cached window bounds from before entering fullscreen.
     *
     * @returns A promise that resolves to the cached {@link Rectangle}.
     */
    getCachedPreFullScreenBoundsAsync() {
        return this.get<Rectangle>("cachedPreFullScreenBounds");
    }

    /**
     * Retrieves the cached window bounds from before entering maximized state.
     *
     * @returns A promise that resolves to the cached {@link Rectangle}.
     */
    getCachedPreMaximizedBoundsAsync() {
        return this.get<Rectangle>("cachedPreMaximizedBounds");
    }

    /**
     * Caches the window bounds before entering fullscreen mode.
     *
     * @param bounds - The {@link Rectangle} to cache.
     */
    setCachedPreFullScreenBounds(bounds: Rectangle) {
        return this.post("setCachedPreFullScreenBounds", {bounds});
    }

    /**
     * Caches the window bounds before entering maximized state.
     *
     * @param bounds - The {@link Rectangle} to cache.
     */
    setCachedPreMaximizedBounds(bounds: Rectangle) {
        return this.post("setCachedPreMaximizedBounds", {bounds});
    }

    /**
     * Sets or clears the maximized state of the window.
     *
     * @param maximized - Whether the window should be maximized. Defaults to `true`.
     */
    setMaximized(maximized = true) {
        return this.post("setMaximized", {maximized});
    }

    /**
     * Toggles the maximized state of the window.
     */
    toggleMaximized() {
        return this.post("toggleMaximized");
    }

    /**
     * Sets or clears the minimized state of the window.
     *
     * @param minimized - Whether the window should be minimized. Defaults to `true`.
     */
    setMinimized(minimized = true) {
        return this.post("setMinimized", {minimized});
    }

    /**
     * Sets or clears fullscreen mode.
     *
     * @param fullScreen - Whether the window should enter fullscreen. Defaults to `true`.
     */
    setFullScreen(fullScreen = true) {
        return this.post("setFullScreen", {fullScreen});
    }

    /**
     * Brings the window to focus.
     */
    setFocused() {
        return this.post("setFocused");
    }

    /**
     * Sets the zoom factor for the window.
     *
     * @param zoom - The zoom factor (e.g. `1.0` for 100%, `1.5` for 150%).
     */
    setZoomFactor(zoom: number) {
        return this.post("setZoomFactor", {zoom});
    }

    /**
     * Enables or disables zoom for the window.
     *
     * @param enabled - Whether zoom should be enabled. Defaults to `true`.
     */
    enableZoom(enabled = true) {
        return this.post("enableZoom", {enabled});
    }

    /**
     * Sets or clears the topmost (always on top) state.
     *
     * @param topMost - Whether the window should be topmost. Defaults to `true`.
     */
    setTopMost(topMost = true) {
        return this.post("setTopMost", {topMost});
    }
}
