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
import type {WindowChrome} from "./Window/WindowChrome";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements InfiniFrameContract {
    host?: InfiniFrameHostBridge;
    setup?: InfiniFrameSetup;
    messaging: InfiniFrameHostMessagingContract;
    window: InfiniFrameWindowContract;
    utils: InfiniFrameUtilsContract;
    windowChrome?: WindowChrome;

    constructor(existing?: Partial<InfiniFrameContract>) {
        this.host = existing?.host;
        this.setup = existing?.setup;
        this.messaging = existing?.messaging ?? new InfiniFrameHostMessaging();
        this.window = existing?.window?.features ? existing.window : new InfiniFrameWindow();
        this.utils = existing?.utils ?? new InfiniFrameUtils();
        this.windowChrome = existing?.windowChrome;
    }
}

export default InfiniFrame
