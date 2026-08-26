/**
 * Utility type contracts.
 * @module InfiniFrameUtils
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * DOM utility helpers provided by the InfiniFrame runtime.
 */
export interface InfiniFrameUtils {
    /**
     * Captures pointer events on the specified element for the given pointer ID.
     * @param element - The DOM element to capture pointer events on.
     * @param pointerId - The pointer ID to capture.
     */
    setPointerCapture(element: Element, pointerId: number): void;

    /**
     * Releases a previously captured pointer from the specified element.
     * @param element - The DOM element to release the pointer from.
     * @param pointerId - The pointer ID to release.
     */
    releasePointerCapture(element: Element, pointerId: number): void;
}
