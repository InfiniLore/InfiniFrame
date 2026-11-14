/**
 * Lifecycle feature contract. Defines the JS API for window lifecycle state queries and close operations.
 * @module LifecycleInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {WindowLifecycleState} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Window lifecycle feature API for the InfiniFrame window.
 * Provides methods to query the current lifecycle state and request window closure.
 */
export interface LifecycleInfiniFrameWindowFeature {
    /**
     * Gets the current lifecycle state of the window.
     * Note: WaitForClose cannot block the web-message/UI thread. A future JS wait API must be event-backed.
     * @returns A promise resolving to the current lifecycle state.
     */
    getStateAsync(): Promise<WindowLifecycleState>;

    /**
     * Checks whether the window is closed or in the process of closing.
     * @returns A promise resolving to true if the window is closed or closing.
     */
    isClosedOrClosingAsync(): Promise<boolean>;

    /**
     * Requests the window to close. The actual close may be asynchronous.
     */
    close(): void;
}
