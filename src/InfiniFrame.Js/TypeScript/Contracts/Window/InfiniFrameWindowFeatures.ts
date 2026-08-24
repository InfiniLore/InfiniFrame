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
export interface InfiniFrameWindowFeatures {
    browser: BrowserInfiniFrameWindowFeature
    debugging: DebuggingInfiniFrameWindowFeature
    decorations: DecorationsInfiniFrameWindowFeature
    filePickerDialogs: FilePickerDialogsInfiniFrameWindowFeature
    invoke: InvokeInfiniFrameWindowFeature
    lifecycle: LifecycleInfiniFrameWindowFeature
    monitors: MonitorsInfiniFrameWindowFeature
    notifications: NotificationsInfiniFrameWindowFeature
    pageNavigation: PageNavigationInfiniFrameWindowFeature
    position: PositionInfiniFrameWindowFeature
    size: SizeInfiniFrameWindowFeature
    state: StateInfiniFrameWindowFeature
    webMessaging: WebMessagingInfiniFrameWindowFeature
}
