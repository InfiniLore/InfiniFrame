/**
 * Position feature contract. Defines the JS API for getting and setting the window's
 * screen position, centering, and monitor-aware placement.
 * @module PositionInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Point} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Window position feature API for the InfiniFrame window.
 * Provides methods to get, set, offset, and center the window on screen.
 */
export interface PositionInfiniFrameWindowFeature {
    /**
     * Gets the current window position as a point.
     * @returns A promise resolving to the window's top-left corner coordinates.
     */
    getLocationAsync(): Promise<Point>;

    /**
     * Gets the current top (Y) coordinate of the window.
     * @returns A promise resolving to the top coordinate in pixels.
     */
    getTopAsync(): Promise<number>;

    /**
     * Gets the current left (X) coordinate of the window.
     * @returns A promise resolving to the left coordinate in pixels.
     */
    getLeftAsync(): Promise<number>;

    /**
     * Sets the window position.
     * @param left - The new left (X) coordinate in pixels.
     * @param top - The new top (Y) coordinate in pixels.
     */
    setLocation(left: number, top: number): void;

    /**
     * Sets the window's left (X) coordinate.
     * @param left - The new left coordinate in pixels.
     */
    setLeft(left: number): void;

    /**
     * Sets the window's top (Y) coordinate.
     * @param top - The new top coordinate in pixels.
     */
    setTop(top: number): void;

    /**
     * Offsets the window position by the specified amount.
     * @param left - Horizontal offset in pixels (positive = right).
     * @param top - Vertical offset in pixels (positive = down).
     */
    offset(left: number, top: number): void;

    /**
     * Centers the window on the primary monitor.
     */
    center(): void;

    /**
     * Centers the window on the monitor currently containing the window.
     */
    centerOnCurrentMonitor(): void;

    /**
     * Centers the window on the specified monitor.
     * @param monitorIndex - Zero-based index of the monitor to center on.
     */
    centerOnMonitor(monitorIndex: number): void;

    /**
     * Moves the window to the specified position, clamped within the current monitor's work area.
     * @param left - Desired left coordinate in pixels.
     * @param top - Desired top coordinate in pixels.
     */
    moveWithinCurrentMonitorArea(left: number, top: number): void;
}
