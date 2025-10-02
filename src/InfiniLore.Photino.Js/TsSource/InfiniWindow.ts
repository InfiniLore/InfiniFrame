// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniWindow} from "./Contracts/IInfiniWindow";
import {IHostMessaging} from "./Contracts/IHostMessaging";
import {HostMessaging} from "./HostMessaging";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniWindow implements IInfiniWindow {
    HostMessaging : IHostMessaging = new HostMessaging();
    
    setPointerCapture(element:Element, pointerId: number) {
        element.setPointerCapture(pointerId);
    }

    releasePointerCapture(element:Element, pointerId:number) {
        element.releasePointerCapture(pointerId);
    }
}

export default InfiniWindow