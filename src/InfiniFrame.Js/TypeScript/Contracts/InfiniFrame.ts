// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameHostMessaging} from "./InfiniFrameHostMessaging";
import type {InfiniFrameUtils} from "./InfiniFrameUtils";
import type {InfiniFrameWindow} from "./Window/InfiniFrameWindow";
import type {InfiniFrameHostBridge} from "./InfiniFrameHostBridge";
import type {InfiniFrameSetup} from "./InfiniFrameSetup";
import type {WindowChrome} from "../Window/WindowChrome";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrame {
    host?: InfiniFrameHostBridge;
    setup?: InfiniFrameSetup;
    messaging: InfiniFrameHostMessaging;
    window: InfiniFrameWindow;
    utils: InfiniFrameUtils;
    windowChrome?: WindowChrome;
}
