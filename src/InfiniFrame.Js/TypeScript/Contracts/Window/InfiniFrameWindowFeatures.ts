// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    InfiniFrameWindowFeatureBrowser,
    InfiniFrameWindowFeaturePosition,
    InfiniFrameWindowFeatureSize,
    InfiniFrameWindowFeatureState,
    InfiniFrameWindowFeatureWebMessaging,
    InfiniFrameWindowFeatureNotifications,
    InfiniFrameWindowFeatureDebugging,
    InfiniFrameWindowFeatureDecorations,
    InfiniFrameWindowFeatureFilePickerDialogs,
    InfiniFrameWindowFeatureInvoke,
    InfiniFrameWindowFeatureLifecycle,
    InfiniFrameWindowFeatureMonitors,
    InfiniFrameWindowFeaturePageNavigation
} from "./Features";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatures {
    browser : InfiniFrameWindowFeatureBrowser
    debugging: InfiniFrameWindowFeatureDebugging
    decorations: InfiniFrameWindowFeatureDecorations
    filePickerDialogs : InfiniFrameWindowFeatureFilePickerDialogs
    invoke : InfiniFrameWindowFeatureInvoke
    lifecycle : InfiniFrameWindowFeatureLifecycle
    monitors : InfiniFrameWindowFeatureMonitors
    notifications : InfiniFrameWindowFeatureNotifications
    pageNavigation : InfiniFrameWindowFeaturePageNavigation
    position : InfiniFrameWindowFeaturePosition
    size : InfiniFrameWindowFeatureSize
    state : InfiniFrameWindowFeatureState
    webMessaging : InfiniFrameWindowFeatureWebMessaging
}