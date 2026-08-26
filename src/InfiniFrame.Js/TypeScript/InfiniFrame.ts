/**
 * Main InfiniFrame class. Provides the public API surface for window features, messaging, and window chrome registration.
 * @module InfiniFrame
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    InfiniFrame as InfiniFrameContract,
    InfiniFrameHostBridge,
    InfiniFrameHostMessaging as InfiniFrameHostMessagingContract,
    InfiniFrameSetup,
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

/**
 * Entry point for the InfiniFrame JavaScript API. Accessible via `window.infiniframe`.
 * Aggregates messaging, window features, and utility subsystems behind a single facade.
 */
export class InfiniFrame implements InfiniFrameContract {
    /** Reference to the native host bridge used for postMessage communication. */
    host?: InfiniFrameHostBridge;

    /** Setup guard that tracks initialisation state. */
    setup?: InfiniFrameSetup;

    /** Messaging transport layer for communicating with the C# host. */
    messaging: InfiniFrameHostMessagingContract;

    /** Window feature manager for title, decorations, and other chrome. */
    window: InfiniFrameWindowContract;

    /** General-purpose utility methods. */
    utils: InfiniFrameUtilsContract;

    /** Optional window chrome controller for custom window decorations. */
    windowChrome?: WindowChrome;

    /**
     * Creates a new InfiniFrame instance.
     * @param existing - Optional partial instance whose properties will be used as defaults.
     *                   Missing properties are initialised with fresh instances.
     */
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
