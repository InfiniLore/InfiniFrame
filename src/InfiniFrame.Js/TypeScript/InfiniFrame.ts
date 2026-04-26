// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame} from "./Contracts/IInfiniFrame";
import {IInfiniFrameHostMessaging} from "./Contracts/IInfiniFrameHostMessaging";
import InfiniFrameHostMessaging from "./InfiniFrameHostMessaging";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements IInfiniFrame {
    Host: IInfiniFrameHostMessaging = new InfiniFrameHostMessaging();
    
    setPointerCapture(element: Element, pointerId: number) {
        element.setPointerCapture(pointerId);
    }

    releasePointerCapture(element: Element, pointerId: number) {
        element.releasePointerCapture(pointerId);
    }
}

export default InfiniFrame
