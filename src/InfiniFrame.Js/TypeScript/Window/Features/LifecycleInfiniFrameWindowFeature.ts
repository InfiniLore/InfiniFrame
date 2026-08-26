/**
 * Window lifecycle feature. Provides close, ready-state, and teardown management.
 *
 * @module LifecycleInfiniFrameWindowFeature
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {LifecycleInfiniFrameWindowFeature as Contract, WindowLifecycleState} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Provides access to the window lifecycle state and allows initiating window close.
 */
export class LifecycleInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    /**
     * Creates a new lifecycle feature instance.
     */
    constructor() {
        super("lifecycle");
    }

    /**
     * Retrieves the current lifecycle state of the window.
     *
     * @returns A promise that resolves to the {@link WindowLifecycleState} value.
     */
    getStateAsync() {
        return this.get<WindowLifecycleState>("state");
    }

    /**
     * Checks whether the window is in the closed or closing state.
     *
     * @returns A promise that resolves to `true` if the window is closed or closing.
     */
    isClosedOrClosingAsync() {
        return this.get<boolean>("isClosedOrClosing");
    }

    /**
     * Initiates an asynchronous window close. The host processes the close request
     * and may trigger the teardown lifecycle.
     */
    close() {
        return this.post("close");
    }
}
