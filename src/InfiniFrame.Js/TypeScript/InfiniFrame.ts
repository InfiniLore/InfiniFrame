// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    InfiniFrame as InfiniFrameContract,
    InfiniFrameHostBridge,
    InfiniFrameSetup,
    InfiniFrameHostMessaging as InfiniFrameHostMessagingContract,
    InfiniFrameUtils as InfiniFrameUtilsContract,
    InfiniFrameWindow as InfiniFrameWindowContract
} from "./Contracts";
import InfiniFrameHostMessaging from "./InfiniFrameHostMessaging";
import {InfiniFrameUtils} from "./InfiniFrameUtils";
import {InfiniFrameWindow} from "./Window/InfiniFrameWindow";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements InfiniFrameContract {
    host?: InfiniFrameHostBridge;
    setup?: InfiniFrameSetup;
    messaging: InfiniFrameHostMessagingContract;
    window: InfiniFrameWindowContract;
    utils: InfiniFrameUtilsContract;

    constructor(existing?: Partial<InfiniFrameContract>) {
        this.host = existing?.host;
        this.setup = existing?.setup;
        this.messaging = existing?.messaging ?? new InfiniFrameHostMessaging();
        this.window = existing?.window ?? new InfiniFrameWindow();
        this.utils = existing?.utils ?? new InfiniFrameUtils();
    }
}

export default InfiniFrame
