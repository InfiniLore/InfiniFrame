/**
 * Size feature contract. Defines the JS API for getting and setting the window's
 * dimensions, minimum/maximum constraints, resize operations, and resizable flag.
 * @module SizeInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {ResizeOrigin, Size} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Window size feature API for the InfiniFrame window.
 * Provides methods to get, set, constrain, and resize the window.
 */
export interface SizeInfiniFrameWindowFeature {
    /**
     * Gets the current window size.
     * @returns A promise resolving to the window's width and height.
     */
    getSizeAsync(): Promise<Size>;

    /**
     * Gets the current window height.
     * @returns A promise resolving to the height in pixels.
     */
    getHeightAsync(): Promise<number>;

    /**
     * Gets the current window width.
     * @returns A promise resolving to the width in pixels.
     */
    getWidthAsync(): Promise<number>;

    /**
     * Gets the maximum allowed window size.
     * @returns A promise resolving to the maximum width and height.
     */
    getMaxSizeAsync(): Promise<Size>;

    /**
     * Gets the maximum allowed window height.
     * @returns A promise resolving to the maximum height in pixels.
     */
    getMaxHeightAsync(): Promise<number>;

    /**
     * Gets the maximum allowed window width.
     * @returns A promise resolving to the maximum width in pixels.
     */
    getMaxWidthAsync(): Promise<number>;

    /**
     * Gets the minimum allowed window size.
     * @returns A promise resolving to the minimum width and height.
     */
    getMinSizeAsync(): Promise<Size>;

    /**
     * Gets the minimum allowed window height.
     * @returns A promise resolving to the minimum height in pixels.
     */
    getMinHeightAsync(): Promise<number>;

    /**
     * Gets the minimum allowed window width.
     * @returns A promise resolving to the minimum width in pixels.
     */
    getMinWidthAsync(): Promise<number>;

    /**
     * Checks whether the window is resizable by the user.
     * @returns A promise resolving to true if the window can be resized.
     */
    isResizableAsync(): Promise<boolean>;

    /**
     * Sets the window size.
     * @param width - New width in pixels.
     * @param height - New height in pixels.
     */
    setSize(width: number, height: number): void;

    /**
     * Sets the window height.
     * @param height - New height in pixels.
     */
    setHeight(height: number): void;

    /**
     * Sets the window width.
     * @param width - New width in pixels.
     */
    setWidth(width: number): void;

    /**
     * Sets the maximum allowed window size.
     * @param width - Maximum width in pixels.
     * @param height - Maximum height in pixels.
     */
    setMaxSize(width: number, height: number): void;

    /**
     * Sets the maximum allowed window height.
     * @param height - Maximum height in pixels.
     */
    setMaxHeight(height: number): void;

    /**
     * Sets the maximum allowed window width.
     * @param width - Maximum width in pixels.
     */
    setMaxWidth(width: number): void;

    /**
     * Sets the minimum allowed window size.
     * @param width - Minimum width in pixels.
     * @param height - Minimum height in pixels.
     */
    setMinSize(width: number, height: number): void;

    /**
     * Sets the minimum allowed window height.
     * @param height - Minimum height in pixels.
     */
    setMinHeight(height: number): void;

    /**
     * Sets the minimum allowed window width.
     * @param width - Minimum width in pixels.
     */
    setMinWidth(width: number): void;

    /**
     * Resizes the window by an offset from the specified origin edge.
     * @param widthOffset - Horizontal offset in pixels.
     * @param heightOffset - Vertical offset in pixels.
     * @param origin - Which edge or corner to resize from.
     */
    resize(widthOffset: number, heightOffset: number, origin: ResizeOrigin): void;

    /**
     * Enables or disables user resizing of the window.
     * @param resizable - true to allow resizing, false to lock the size. Defaults to true.
     */
    setResizable(resizable?: boolean): void;
}
