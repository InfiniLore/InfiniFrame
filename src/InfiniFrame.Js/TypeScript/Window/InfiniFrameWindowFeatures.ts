// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {
    BrowserInfiniFrameWindowFeature as InfiniFrameWindowFeatureBrowserContract,
    DebuggingInfiniFrameWindowFeature as InfiniFrameWindowFeatureDebuggingContract,
    DecorationsInfiniFrameWindowFeature as InfiniFrameWindowFeatureDecorationsContract,
    FilePickerDialogsInfiniFrameWindowFeature as InfiniFrameWindowFeatureFilePickerDialogsContract,
    InvokeInfiniFrameWindowFeature as InfiniFrameWindowFeatureInvokeContract,
    LifecycleInfiniFrameWindowFeature as InfiniFrameWindowFeatureLifecycleContract,
    MonitorsInfiniFrameWindowFeature as InfiniFrameWindowFeatureMonitorsContract,
    NotificationsInfiniFrameWindowFeature as InfiniFrameWindowFeatureNotificationsContract,
    PageNavigationInfiniFrameWindowFeature as InfiniFrameWindowFeaturePageNavigationContract,
    PositionInfiniFrameWindowFeature as InfiniFrameWindowFeaturePositionContract,
    SizeInfiniFrameWindowFeature as InfiniFrameWindowFeatureSizeContract,
    StateInfiniFrameWindowFeature as InfiniFrameWindowFeatureStateContract,
    WebMessagingInfiniFrameWindowFeature as InfiniFrameWindowFeatureWebMessagingContract,
    InfiniFrameWindowFeatures as InfiniFrameWindowFeaturesContract
} from "../Contracts";
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
    }
    
}
