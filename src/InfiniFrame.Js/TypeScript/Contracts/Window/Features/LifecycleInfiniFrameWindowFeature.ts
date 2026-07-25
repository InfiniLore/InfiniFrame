// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {WindowLifecycleState} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface LifecycleInfiniFrameWindowFeature {
    // WaitForClose cannot block the web-message/UI thread. A future JS wait API must be an event-backed Promise.
    getStateAsync(): Promise<WindowLifecycleState>;
    isClosedOrClosingAsync(): Promise<boolean>;
    close(): void;
}
