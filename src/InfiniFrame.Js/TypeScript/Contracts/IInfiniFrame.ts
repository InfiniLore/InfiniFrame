// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniFrameHostMessaging} from "./IInfiniFrameHostMessaging";
import {IInfiniFrameUtils} from "./IInfiniFrameUtils";
import {IInfiniFrameWindow} from "./IInfiniFrameWindow";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface IInfiniFrame {
    messaging: IInfiniFrameHostMessaging;
    window: IInfiniFrameWindow;
    utils: IInfiniFrameUtils;
}
