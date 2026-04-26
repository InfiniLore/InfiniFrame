// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameHostMessaging} from "./IInfiniFrameHostMessaging";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface IInfiniFrame {
    Host: IInfiniFrameHostMessaging;

    setPointerCapture(element: Element, pointerId: number): void;

    releasePointerCapture(element: Element, pointerId: number): void;
}
