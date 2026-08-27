/**
 * Display enumeration and DPI feature. Provides information about connected monitors
 * and screen DPI.
 *
 * @module MonitorsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniMonitor, MonitorsInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to display monitor enumeration and screen DPI information.
 */
export class MonitorsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new monitors feature instance.
     */
    constructor() {
        super("monitors");
    }

    /**
     * Retrieves information about all connected display monitors.
     *
     * @returns A promise that resolves to an array of {@link InfiniMonitor} objects.
     */
    getMonitorsAsync() {
        return this.get<InfiniMonitor[]>("monitors");
    }

    /**
     * Retrieves information about the primary (main) display monitor.
     *
     * @returns A promise that resolves to the {@link InfiniMonitor} for the main display.
     */
    getMainMonitorAsync() {
        return this.get<InfiniMonitor>("mainMonitor");
    }

    /**
     * Retrieves the screen DPI of the main display monitor.
     *
     * @returns A promise that resolves to the DPI value as a number.
     */
    getMainMonitorScreenDpiAsync() {
        return this.get<number>("mainMonitorScreenDpi");
    }
}
