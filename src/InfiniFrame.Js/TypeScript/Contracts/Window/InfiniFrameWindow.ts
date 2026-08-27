/**
 * Window facade contract. Defines the public API for the InfiniFrameWindow JavaScript class.
 * @module InfiniFrameWindow
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {InfiniFrameWindowFeatures} from "./InfiniFrameWindowFeatures";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Public API surface for the InfiniFrame window instance.
 * Provides access to all window feature groups via the {@link features} property.
 */
export interface InfiniFrameWindow {
    /** Collection of all window feature groups (browser, position, size, state, etc.). */
    features: InfiniFrameWindowFeatures
}
