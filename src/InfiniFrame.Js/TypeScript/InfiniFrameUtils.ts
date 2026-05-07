// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameUtils} from "./Contracts";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameUtils implements InfiniFrameUtils {
    setPointerCapture(element: Element, pointerId: number): void {
        if (element === null) return;
        if (pointerId === null) return;
        
        if (element.hasPointerCapture(pointerId)) return;
        element.setPointerCapture(pointerId);
    }
    
    releasePointerCapture(element: Element, pointerId: number): void {
        if (element === null) return;
        if (pointerId === null) return;
        
        if (!element.hasPointerCapture(pointerId)) return;
        element.releasePointerCapture(pointerId);
    }
}
