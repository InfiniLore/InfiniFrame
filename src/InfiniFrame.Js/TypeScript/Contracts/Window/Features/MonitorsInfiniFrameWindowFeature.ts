/**
 * Monitors feature contract. Defines the JS API for enumerating display monitors
 * and querying screen properties.
 * @module MonitorsInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniMonitor} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Monitor enumeration feature API for the InfiniFrame window.
 * Provides methods to query connected displays and their properties.
 */
export interface MonitorsInfiniFrameWindowFeature {
    /**
     * Gets information about all connected monitors.
     * @returns A promise resolving to an array of monitor descriptors.
     */
    getMonitorsAsync(): Promise<InfiniMonitor[]>;

    /**
     * Gets information about the primary monitor.
     * @returns A promise resolving to the primary monitor descriptor.
     */
    getMainMonitorAsync(): Promise<InfiniMonitor>;

    /**
     * Gets the DPI (dots per inch) of the primary monitor.
     * @returns A promise resolving to the DPI value.
     */
    getMainMonitorScreenDpiAsync(): Promise<number>;
}
