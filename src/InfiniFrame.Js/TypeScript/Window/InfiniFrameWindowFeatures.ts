/**
 * Feature collection. Instantiates and exposes all window feature implementations.
 *
 * @module InfiniFrameWindowFeatures
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    BrowserInfiniFrameWindowFeature as InfiniFrameWindowFeatureBrowserContract,
    DebuggingInfiniFrameWindowFeature as InfiniFrameWindowFeatureDebuggingContract,
    DecorationsInfiniFrameWindowFeature as InfiniFrameWindowFeatureDecorationsContract,
    FilePickerDialogsInfiniFrameWindowFeature as InfiniFrameWindowFeatureFilePickerDialogsContract,
    InfiniFrameWindowFeatures as InfiniFrameWindowFeaturesContract,
    InvokeInfiniFrameWindowFeature as InfiniFrameWindowFeatureInvokeContract,
    JavaScriptInfiniFrameWindowFeature as InfiniFrameWindowFeatureJavaScriptContract,
    LifecycleInfiniFrameWindowFeature as InfiniFrameWindowFeatureLifecycleContract,
    MonitorsInfiniFrameWindowFeature as InfiniFrameWindowFeatureMonitorsContract,
    NotificationsInfiniFrameWindowFeature as InfiniFrameWindowFeatureNotificationsContract,
    PageNavigationInfiniFrameWindowFeature as InfiniFrameWindowFeaturePageNavigationContract,
    PositionInfiniFrameWindowFeature as InfiniFrameWindowFeaturePositionContract,
    SizeInfiniFrameWindowFeature as InfiniFrameWindowFeatureSizeContract,
    StateInfiniFrameWindowFeature as InfiniFrameWindowFeatureStateContract,
    WebMessagingInfiniFrameWindowFeature as InfiniFrameWindowFeatureWebMessagingContract
} from "../Contracts";
import {
    BrowserInfiniFrameWindowFeature,
    DebuggingInfiniFrameWindowFeature,
    DecorationsInfiniFrameWindowFeature,
    FilePickerDialogsInfiniFrameWindowFeature,
    InvokeInfiniFrameWindowFeature,
    JavaScriptInfiniFrameWindowFeature,
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
 * Container for all runtime window features (Browser, Debugging, Decorations, Size, State, Position, etc.).
 */
export class InfiniFrameWindowFeatures implements InfiniFrameWindowFeaturesContract {
    /** Browser settings: context menu, media autoplay, web security, user agent. */
    browser: InfiniFrameWindowFeatureBrowserContract;
    /** Debug settings: dev tools, remote debugging, diagnostics. */
    debugging: InfiniFrameWindowFeatureDebuggingContract;
    /** Window decorations: title, icon, transparency, background color. */
    decorations: InfiniFrameWindowFeatureDecorationsContract;
    /** Native file and folder picker dialogs. */
    filePickerDialogs: InfiniFrameWindowFeatureFilePickerDialogsContract;
    /** Cross-thread invoke dispatch. */
    invoke: InfiniFrameWindowFeatureInvokeContract;
    /** Window lifecycle: close, ready, teardown. */
    lifecycle: InfiniFrameWindowFeatureLifecycleContract;
    /** Display enumeration and DPI information. */
    monitors: InfiniFrameWindowFeatureMonitorsContract;
    /** Desktop notifications and message box dialogs. */
    notifications: InfiniFrameWindowFeatureNotificationsContract;
    /** URL and HTML page navigation. */
    pageNavigation: InfiniFrameWindowFeaturePageNavigationContract;
    /** Window position and centering. */
    position: InfiniFrameWindowFeaturePositionContract;
    /** Window dimensions and constraints. */
    size: InfiniFrameWindowFeatureSizeContract;
    /** Window state: maximized, minimized, fullscreen, topmost, zoom. */
    state: InfiniFrameWindowFeatureStateContract;
    /** Web messaging to C# host. */
    webMessaging: InfiniFrameWindowFeatureWebMessagingContract;
    /** JavaScript execution in the browser context. */
    javaScript: InfiniFrameWindowFeatureJavaScriptContract;

    /**
     * Creates a new feature collection, instantiating every window feature.
     */
    constructor() {
        this.browser = new BrowserInfiniFrameWindowFeature();
        this.debugging = new DebuggingInfiniFrameWindowFeature();
        this.decorations = new DecorationsInfiniFrameWindowFeature();
        this.filePickerDialogs = new FilePickerDialogsInfiniFrameWindowFeature();
        this.invoke = new InvokeInfiniFrameWindowFeature();
        this.lifecycle = new LifecycleInfiniFrameWindowFeature();
        this.monitors = new MonitorsInfiniFrameWindowFeature();
        this.notifications = new NotificationsInfiniFrameWindowFeature();
        this.pageNavigation = new PageNavigationInfiniFrameWindowFeature();
        this.position = new PositionInfiniFrameWindowFeature();
        this.size = new SizeInfiniFrameWindowFeature();
        this.state = new StateInfiniFrameWindowFeature();
        this.webMessaging = new WebMessagingInfiniFrameWindowFeature();
        this.javaScript = new JavaScriptInfiniFrameWindowFeature();
    }

}
