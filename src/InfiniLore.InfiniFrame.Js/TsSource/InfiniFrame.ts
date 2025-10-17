// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame} from "./Contracts/IInfiniFrame";
import {IHostMessaging, SendToHostMessageId} from "./Contracts/IHostMessaging";
import HostMessaging from "./HostMessaging";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements IInfiniFrame {
    HostMessaging: IHostMessaging = new HostMessaging();

    // Overload to make a dev's life easier instead of having to go to the HostMessaging class
    sendMessageToHost(id: SendToHostMessageId, data?: string) {
        this.HostMessaging.sendMessageToHost(id, data);
    }

    setPointerCapture(element: Element, pointerId: number) {
        element.setPointerCapture(pointerId);
    }

    releasePointerCapture(element: Element, pointerId: number) {
        element.releasePointerCapture(pointerId);
    }
}

export default InfiniFrame