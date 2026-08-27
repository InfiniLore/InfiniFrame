/**
 * Window facade class. Wraps the messaging bridge and provides access to window features.
 *
 * @module InfiniFrameWindow
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindow as InfiniFrameWindowContract} from "../Contracts";
import {InfiniFrameWindowFeatures} from "./InfiniFrameWindowFeatures";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Represents a native window and provides access to its features and messaging capabilities.
 */
export class InfiniFrameWindow implements InfiniFrameWindowContract {
    /** The collection of all window feature implementations (browser, debugging, decorations, etc.). */
    readonly features = new InfiniFrameWindowFeatures();
}
