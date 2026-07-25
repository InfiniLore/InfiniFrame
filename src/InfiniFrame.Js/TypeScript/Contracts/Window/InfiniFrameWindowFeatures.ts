// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {
    BrowserInfiniFrameWindowFeature,
    PositionInfiniFrameWindowFeature,
    SizeInfiniFrameWindowFeature,
    StateInfiniFrameWindowFeature,
    WebMessagingInfiniFrameWindowFeature,
    NotificationsInfiniFrameWindowFeature,
    DebuggingInfiniFrameWindowFeature,
    DecorationsInfiniFrameWindowFeature,
    FilePickerDialogsInfiniFrameWindowFeature,
    InvokeInfiniFrameWindowFeature,
    LifecycleInfiniFrameWindowFeature,
    MonitorsInfiniFrameWindowFeature,
    PageNavigationInfiniFrameWindowFeature
} from "./Features";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatures {
    browser : BrowserInfiniFrameWindowFeature
    debugging: DebuggingInfiniFrameWindowFeature
    decorations: DecorationsInfiniFrameWindowFeature
    filePickerDialogs : FilePickerDialogsInfiniFrameWindowFeature
    invoke : InvokeInfiniFrameWindowFeature
    lifecycle : LifecycleInfiniFrameWindowFeature
    monitors : MonitorsInfiniFrameWindowFeature
    notifications : NotificationsInfiniFrameWindowFeature
    pageNavigation : PageNavigationInfiniFrameWindowFeature
    position : PositionInfiniFrameWindowFeature
    size : SizeInfiniFrameWindowFeature
    state : StateInfiniFrameWindowFeature
    webMessaging : WebMessagingInfiniFrameWindowFeature
}