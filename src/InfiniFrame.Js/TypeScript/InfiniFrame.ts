// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    InfiniFrame,
    InfiniFrameHostBridge,
    InfiniFrameHostMessaging,
    InfiniFrameUtils,
    InfiniFrameWindow
} from "./Contracts";
import InfiniFrameHostMessaging from "./InfiniFrameHostMessaging";
import {InfiniFrameUtils} from "./InfiniFrameUtils";
import {InfiniFrameWindow} from "./InfiniFrameWindow";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrame implements InfiniFrame {
    host?: InfiniFrameHostBridge;
    setup?: InfiniFrame["setup"];
    messaging: InfiniFrameHostMessaging;
    window: InfiniFrameWindow;
    utils: InfiniFrameUtils;

    constructor(existing?: Partial<InfiniFrame>) {
        this.host = existing?.host;
        this.setup = existing?.setup;
        this.messaging = existing?.messaging ?? new InfiniFrameHostMessaging();
        this.window = existing?.window ?? new InfiniFrameWindow();
        this.utils = existing?.utils ?? new InfiniFrameUtils();
    }
}

export default InfiniFrame
