// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    InfiniFrameWindowFeatureBrowser as InfiniFrameWindowFeatureBrowserContract,
    InfiniFrameWindowFeatureDebugging as InfiniFrameWindowFeatureDebuggingContract,
    InfiniFrameWindowFeatureDecorations as InfiniFrameWindowFeatureDecorationsContract,
    InfiniFrameWindowFeatureFilePickerDialogs as InfiniFrameWindowFeatureFilePickerDialogsContract,
    InfiniFrameWindowFeatureInvoke as InfiniFrameWindowFeatureInvokeContract,
    InfiniFrameWindowFeatureLifecycle as InfiniFrameWindowFeatureLifecycleContract,
    InfiniFrameWindowFeatureMonitors as InfiniFrameWindowFeatureMonitorsContract,
    InfiniFrameWindowFeatureNotifications as InfiniFrameWindowFeatureNotificationsContract,
    InfiniFrameWindowFeaturePageNavigation as InfiniFrameWindowFeaturePageNavigationContract,
    InfiniFrameWindowFeaturePosition as InfiniFrameWindowFeaturePositionContract,
    InfiniFrameWindowFeatureSize as InfiniFrameWindowFeatureSizeContract,
    InfiniFrameWindowFeatureState as InfiniFrameWindowFeatureStateContract,
    InfiniFrameWindowFeatureWebMessaging as InfiniFrameWindowFeatureWebMessagingContract,
    InfiniFrameWindowFeatures as InfiniFrameWindowFeaturesContract
} from "../Contracts";
import {
    InfiniFrameWindowFeatureBrowser,
    InfiniFrameWindowFeatureDebugging,
    InfiniFrameWindowFeatureDecorations,
    InfiniFrameWindowFeatureFilePickerDialogs,
    InfiniFrameWindowFeatureInvoke,
    InfiniFrameWindowFeatureLifecycle,
    InfiniFrameWindowFeatureMonitors,
    InfiniFrameWindowFeatureNotifications,
    InfiniFrameWindowFeaturePageNavigation,
    InfiniFrameWindowFeaturePosition,
    InfiniFrameWindowFeatureSize,
    InfiniFrameWindowFeatureState,
    InfiniFrameWindowFeatureWebMessaging
} from "./Features";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeatures implements InfiniFrameWindowFeaturesContract {
    browser: InfiniFrameWindowFeatureBrowserContract;
    debugging: InfiniFrameWindowFeatureDebuggingContract;
    decorations: InfiniFrameWindowFeatureDecorationsContract;
    filePickerDialogs: InfiniFrameWindowFeatureFilePickerDialogsContract;
    invoke: InfiniFrameWindowFeatureInvokeContract;
    lifecycle: InfiniFrameWindowFeatureLifecycleContract;
    monitors: InfiniFrameWindowFeatureMonitorsContract;
    notifications: InfiniFrameWindowFeatureNotificationsContract;
    pageNavigation: InfiniFrameWindowFeaturePageNavigationContract;
    position: InfiniFrameWindowFeaturePositionContract;
    size: InfiniFrameWindowFeatureSizeContract;
    state: InfiniFrameWindowFeatureStateContract;
    webMessaging: InfiniFrameWindowFeatureWebMessagingContract;

    constructor() {
        this.browser = new InfiniFrameWindowFeatureBrowser();
        this.debugging = new InfiniFrameWindowFeatureDebugging();
        this.decorations = new InfiniFrameWindowFeatureDecorations();
        this.filePickerDialogs = new InfiniFrameWindowFeatureFilePickerDialogs();
        this.invoke = new InfiniFrameWindowFeatureInvoke();
        this.lifecycle = new InfiniFrameWindowFeatureLifecycle();
        this.monitors = new InfiniFrameWindowFeatureMonitors();
        this.notifications = new InfiniFrameWindowFeatureNotifications();
        this.pageNavigation = new InfiniFrameWindowFeaturePageNavigation();
        this.position = new InfiniFrameWindowFeaturePosition();
        this.size = new InfiniFrameWindowFeatureSize();
        this.state = new InfiniFrameWindowFeatureState();
        this.webMessaging = new InfiniFrameWindowFeatureWebMessaging();
    }
    
}