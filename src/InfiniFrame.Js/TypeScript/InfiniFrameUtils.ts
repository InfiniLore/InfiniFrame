/**
 * Utility helpers for the InfiniFrame JavaScript library.
 * @module InfiniFrameUtils
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameUtils as InfiniFrameUtilsContract} from "./Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Static utility methods used throughout the InfiniFrame JS library.
 */
export class InfiniFrameUtils implements InfiniFrameUtilsContract {
    /**
     * Captures a pointer on the given element so that subsequent pointer events
     * are directed to that element regardless of where the pointer moves.
     * @param element - The DOM element to capture the pointer on.
     * @param pointerId - The pointer identifier returned by pointer events.
     */
    setPointerCapture(element: Element, pointerId: number): void {
        if (element === null) return;
        if (pointerId === null) return;

        if (element.hasPointerCapture(pointerId)) return;
        element.setPointerCapture(pointerId);
    }

    /**
     * Releases a previously captured pointer from the given element.
     * @param element - The DOM element that currently holds the pointer capture.
     * @param pointerId - The pointer identifier to release.
     */
    releasePointerCapture(element: Element, pointerId: number): void {
        if (element === null) return;
        if (pointerId === null) return;

        if (!element.hasPointerCapture(pointerId)) return;
        element.releasePointerCapture(pointerId);
    }
}
