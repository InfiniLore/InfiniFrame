/**
 * Window size feature. Manages dimensions, min/max constraints, and resizable state.
 *
 * @module SizeInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {ResizeOrigin, Size, SizeInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to window size, min/max constraints, and resize behavior control.
 */
export class SizeInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new size feature instance.
     */
    constructor() {
        super("size");
    }

    /**
     * Retrieves the current window size.
     *
     * @returns A promise that resolves to the {@link Size} with `width` and `height`.
     */
    getSizeAsync() {
        return this.get<Size>("size");
    }

    /**
     * Retrieves the current window height.
     *
     * @returns A promise that resolves to the height in pixels.
     */
    getHeightAsync() {
        return this.get<number>("height");
    }

    /**
     * Retrieves the current window width.
     *
     * @returns A promise that resolves to the width in pixels.
     */
    getWidthAsync() {
        return this.get<number>("width");
    }

    /**
     * Retrieves the maximum allowed window size.
     *
     * @returns A promise that resolves to the maximum {@link Size}.
     */
    getMaxSizeAsync() {
        return this.get<Size>("maxSize");
    }

    /**
     * Retrieves the maximum allowed window height.
     *
     * @returns A promise that resolves to the maximum height in pixels.
     */
    getMaxHeightAsync() {
        return this.get<number>("maxHeight");
    }

    /**
     * Retrieves the maximum allowed window width.
     *
     * @returns A promise that resolves to the maximum width in pixels.
     */
    getMaxWidthAsync() {
        return this.get<number>("maxWidth");
    }

    /**
     * Retrieves the minimum allowed window size.
     *
     * @returns A promise that resolves to the minimum {@link Size}.
     */
    getMinSizeAsync() {
        return this.get<Size>("minSize");
    }

    /**
     * Retrieves the minimum allowed window height.
     *
     * @returns A promise that resolves to the minimum height in pixels.
     */
    getMinHeightAsync() {
        return this.get<number>("minHeight");
    }

    /**
     * Retrieves the minimum allowed window width.
     *
     * @returns A promise that resolves to the minimum width in pixels.
     */
    getMinWidthAsync() {
        return this.get<number>("minWidth");
    }

    /**
     * Checks whether the window is resizable.
     *
     * @returns A promise that resolves to `true` if the window can be resized.
     */
    isResizableAsync() {
        return this.get<boolean>("isResizable");
    }

    /**
     * Sets the window size to the specified dimensions.
     *
     * @param width - The new width in pixels.
     * @param height - The new height in pixels.
     */
    setSize(width: number, height: number) {
        return this.post("setSize", {width, height});
    }

    /**
     * Sets the window height.
     *
     * @param height - The new height in pixels.
     */
    setHeight(height: number) {
        return this.post("setHeight", {height});
    }

    /**
     * Sets the window width.
     *
     * @param width - The new width in pixels.
     */
    setWidth(width: number) {
        return this.post("setWidth", {width});
    }

    /**
     * Sets the maximum allowed window size.
     *
     * @param width - The maximum width in pixels.
     * @param height - The maximum height in pixels.
     */
    setMaxSize(width: number, height: number) {
        return this.post("setMaxSize", {width, height});
    }

    /**
     * Sets the maximum allowed window height.
     *
     * @param height - The maximum height in pixels.
     */
    setMaxHeight(height: number) {
        return this.post("setMaxHeight", {height});
    }

    /**
     * Sets the maximum allowed window width.
     *
     * @param width - The maximum width in pixels.
     */
    setMaxWidth(width: number) {
        return this.post("setMaxWidth", {width});
    }

    /**
     * Sets the minimum allowed window size.
     *
     * @param width - The minimum width in pixels.
     * @param height - The minimum height in pixels.
     */
    setMinSize(width: number, height: number) {
        return this.post("setMinSize", {width, height});
    }

    /**
     * Sets the minimum allowed window height.
     *
     * @param height - The minimum height in pixels.
     */
    setMinHeight(height: number) {
        return this.post("setMinHeight", {height});
    }

    /**
     * Sets the minimum allowed window width.
     *
     * @param width - The minimum width in pixels.
     */
    setMinWidth(width: number) {
        return this.post("setMinWidth", {width});
    }

    /**
     * Resizes the window by the given offset from the specified origin edge.
     *
     * @param widthOffset - The horizontal size change in pixels.
     * @param heightOffset - The vertical size change in pixels.
     * @param origin - The edge or corner from which the resize originates.
     */
    resize(widthOffset: number, heightOffset: number, origin: ResizeOrigin) {
        return this.post("resize", {widthOffset, heightOffset, origin});
    }

    /**
     * Enables or disables window resizing.
     *
     * @param resizable - Whether the window should be resizable. Defaults to `true`.
     */
    setResizable(resizable = true) {
        return this.post("setResizable", {resizable});
    }
}
