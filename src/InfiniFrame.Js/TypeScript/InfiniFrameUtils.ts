// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameUtils} from "./Contracts/IInfiniFrameUtils";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameUtils implements IInfiniFrameUtils {
    setPointerCapture(element: Element, pointerId: number): void {
        element.setPointerCapture(pointerId);
    }
    
    releasePointerCapture(element: Element, pointerId: number): void {
        element.releasePointerCapture(pointerId);
    }
}