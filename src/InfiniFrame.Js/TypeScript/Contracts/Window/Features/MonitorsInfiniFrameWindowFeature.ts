// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniMonitor} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface MonitorsInfiniFrameWindowFeature {
    getMonitorsAsync(): Promise<InfiniMonitor[]>;
    getMainMonitorAsync(): Promise<InfiniMonitor>;
    getMainMonitorScreenDpiAsync(): Promise<number>;
}
