/**
 * Window decorations feature. Manages title, icon, transparency, and background color.
 *
 * @module DecorationsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DecorationsInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to window decoration properties including chromeless mode,
 * transparency, background color, title, and icon file.
 */
export class DecorationsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new decorations feature instance.
     */
    constructor() {
        super("decorations");
    }

    /**
     * Checks whether the window is chromeless (no system title bar or borders).
     *
     * @returns A promise that resolves to `true` if the window is chromeless.
     */
    isChromelessAsync() {
        return this.get<boolean>("isChromeless");
    }

    /**
     * Checks whether the window background is transparent.
     *
     * @returns A promise that resolves to `true` if transparency is enabled.
     */
    isTransparentAsync() {
        return this.get<boolean>("isTransparent");
    }

    /**
     * Retrieves the window background color.
     *
     * @returns A promise that resolves to the CSS color string, or `null` if not set.
     */
    backgroundColorAsync() {
        return this.get<string | null>("backgroundColor");
    }

    /**
     * Retrieves the window title.
     *
     * @returns A promise that resolves to the title string, or `null` if not set.
     */
    getTitleAsync() {
        return this.get<string | null>("title");
    }

    /**
     * Retrieves the path to the window icon file.
     *
     * @returns A promise that resolves to the icon file path, or `null` if not set.
     */
    getIconFilePathAsync() {
        return this.get<string | null>("iconFilePath");
    }

    /**
     * Checks whether the Linux window title length is limited.
     *
     * @returns A promise that resolves to `true` if title length limiting is enabled.
     */
    getLimitLinuxWindowTitleLengthAsync() {
        return this.get<boolean>("limitLinuxWindowTitleLength");
    }

    /**
     * Enables or disables window transparency.
     *
     * @param enabled - Whether to enable transparency. Defaults to `true`.
     */
    setTransparent(enabled = true) {
        return this.post("setTransparent", {enabled});
    }

    /**
     * Sets the window background color.
     *
     * @param color - A CSS color string, or `null` to reset to the default.
     */
    setBackgroundColor(color: string | null) {
        return this.post("setBackgroundColor", {color});
    }

    /**
     * Sets the window title.
     *
     * @param title - The title string, or `null` to clear the title.
     */
    setTitle(title: string | null) {
        return this.post("setTitle", {title});
    }

    /**
     * Sets the window icon from a file path.
     *
     * @param iconFilePath - The file system path to the icon image.
     */
    setIconFile(iconFilePath: string) {
        return this.post("setIconFile", {iconFilePath});
    }

    /**
     * Enables or disables Linux window title length limiting.
     *
     * @param enabled - Whether to enable title length limiting. Defaults to `true`.
     */
    setLimitLinuxWindowTitleLength(enabled = true) {
        return this.post("setLimitLinuxWindowTitleLength", {enabled});
    }
}
