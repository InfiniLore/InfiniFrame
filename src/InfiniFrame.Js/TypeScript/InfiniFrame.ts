// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrame, IInfiniFrameHostMessaging, IInfiniFrameUtils, IInfiniFrameWindow} from "./Contracts";
import InfiniFrameHostMessaging from "./InfiniFrameHostMessaging";
import {InfiniFrameUtils} from "./InfiniFrameUtils";
import {InfiniFrameWindow} from "./InfiniFrameWindow";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements IInfiniFrame {
    hostMessaging: IInfiniFrameHostMessaging = new InfiniFrameHostMessaging();
    hostWindow: IInfiniFrameWindow = new InfiniFrameWindow();
    
    utils: IInfiniFrameUtils = new InfiniFrameUtils()    
}

export default InfiniFrame
