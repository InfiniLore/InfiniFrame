/**
 * Window features collection contract. Defines the shape of window.infiniframe.window.features.
 * @module InfiniFrameWindowFeatures
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    BrowserInfiniFrameWindowFeature,
    DebuggingInfiniFrameWindowFeature,
    DecorationsInfiniFrameWindowFeature,
    FilePickerDialogsInfiniFrameWindowFeature,
    InvokeInfiniFrameWindowFeature,
    LifecycleInfiniFrameWindowFeature,
    MonitorsInfiniFrameWindowFeature,
    NotificationsInfiniFrameWindowFeature,
    PageNavigationInfiniFrameWindowFeature,
    PositionInfiniFrameWindowFeature,
    SizeInfiniFrameWindowFeature,
    StateInfiniFrameWindowFeature,
    WebMessagingInfiniFrameWindowFeature
} from "./Features";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * Aggregates all window feature groups available on an InfiniFrame window instance.
 * Each property provides a focused API for a specific window capability.
 */
export interface InfiniFrameWindowFeatures {
    /** Browser-level settings: context menu, media autoplay, user agent, web security, etc. */
    browser: BrowserInfiniFrameWindowFeature
    /** Debugging tools: dev tools, remote debugging endpoint, web inspector, diagnostics. */
    debugging: DebuggingInfiniFrameWindowFeature
    /** Window decorations: title, icon, background color, transparency, chromeless mode. */
    decorations: DecorationsInfiniFrameWindowFeature
    /** Native file picker dialogs: open file, open folder, save file. */
    filePickerDialogs: FilePickerDialogsInfiniFrameWindowFeature
    /** Direct method invocation on the native host (reserved for future use). */
    invoke: InvokeInfiniFrameWindowFeature
    /** Window lifecycle management: state queries, close, and lifecycle events. */
    lifecycle: LifecycleInfiniFrameWindowFeature
    /** Multi-monitor support: enumerate monitors, get DPI, query work areas. */
    monitors: MonitorsInfiniFrameWindowFeature
    /** Notifications and dialog boxes: toast notifications, message dialogs. */
    notifications: NotificationsInfiniFrameWindowFeature
    /** Page navigation: load URIs, load paths, load raw HTML, get current URL. */
    pageNavigation: PageNavigationInfiniFrameWindowFeature
    /** Window position: get/set location, center, offset, and monitor-aware placement. */
    position: PositionInfiniFrameWindowFeature
    /** Window size: get/set dimensions, min/max constraints, resize, and resizable flag. */
    size: SizeInfiniFrameWindowFeature
    /** Window state: fullscreen, maximize, minimize, topmost, focus, zoom. */
    state: StateInfiniFrameWindowFeature
    /** Web messaging: send web messages to the native host. */
    webMessaging: WebMessagingInfiniFrameWindowFeature
}
