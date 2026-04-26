// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameUtils} from "./Contracts/IInfiniFrameUtils";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameUtils implements IInfiniFrameUtils {
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