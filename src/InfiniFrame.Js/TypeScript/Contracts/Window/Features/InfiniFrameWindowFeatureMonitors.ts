// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniMonitor} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface InfiniFrameWindowFeatureMonitors {
    getMonitorsAsync(): Promise<InfiniMonitor[]>;
    getMainMonitorAsync(): Promise<InfiniMonitor>;
    getMainMonitorScreenDpiAsync(): Promise<number>;
}
