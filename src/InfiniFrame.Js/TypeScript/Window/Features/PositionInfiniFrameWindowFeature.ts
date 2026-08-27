/**
 * Window position feature. Provides window location, centering, and multi-monitor positioning.
 *
 * @module PositionInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Point, PositionInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to window position properties and methods for moving, offsetting,
 * and centering the window across monitors.
 */
export class PositionInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new position feature instance.
     */
    constructor() {
        super("position");
    }

    /**
     * Retrieves the window location as a point.
     *
     * @returns A promise that resolves to the {@link Point} with `left` and `top` coordinates.
     */
    getLocationAsync() {
        return this.get<Point>("location");
    }

    /**
     * Retrieves the top (Y) coordinate of the window.
     *
     * @returns A promise that resolves to the top coordinate in pixels.
     */
    getTopAsync() {
        return this.get<number>("top");
    }

    /**
     * Retrieves the left (X) coordinate of the window.
     *
     * @returns A promise that resolves to the left coordinate in pixels.
     */
    getLeftAsync() {
        return this.get<number>("left");
    }

    /**
     * Sets the window location to the specified coordinates.
     *
     * @param left - The new left (X) coordinate in pixels.
     * @param top - The new top (Y) coordinate in pixels.
     */
    setLocation(left: number, top: number) {
        return this.post("setLocation", {left, top});
    }

    /**
     * Sets the left (X) coordinate of the window.
     *
     * @param left - The new left coordinate in pixels.
     */
    setLeft(left: number) {
        return this.post("setLeft", {left});
    }

    /**
     * Sets the top (Y) coordinate of the window.
     *
     * @param top - The new top coordinate in pixels.
     */
    setTop(top: number) {
        return this.post("setTop", {top});
    }

    /**
     * Offsets the window position by the given amounts.
     *
     * @param left - The horizontal offset in pixels.
     * @param top - The vertical offset in pixels.
     */
    offset(left: number, top: number) {
        return this.post("offset", {left, top});
    }

    /**
     * Centers the window on the primary display.
     */
    center() {
        return this.post("center");
    }

    /**
     * Centers the window on the display monitor it currently overlaps.
     */
    centerOnCurrentMonitor() {
        return this.post("centerOnCurrentMonitor");
    }

    /**
     * Centers the window on a specific display monitor by index.
     *
     * @param monitorIndex - The zero-based index of the monitor to center on.
     */
    centerOnMonitor(monitorIndex: number) {
        return this.post("centerOnMonitor", {monitorIndex});
    }

    /**
     * Moves the window to the specified position while clamping within the current monitor bounds.
     *
     * @param left - The desired left (X) coordinate in pixels.
     * @param top - The desired top (Y) coordinate in pixels.
     */
    moveWithinCurrentMonitorArea(left: number, top: number) {
        return this.post("moveWithinCurrentMonitorArea", {left, top});
    }
}
