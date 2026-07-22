// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindowFeatureMonitors as Contract,InfiniMonitor} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class MonitorsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("monitors");}

    getMonitorsAsync() {
        return this.get<InfiniMonitor[]>("monitors");
    }

    getMainMonitorAsync() {
        return this.get<InfiniMonitor>("mainMonitor");
    }

    getMainMonitorScreenDpiAsync() {
        return this.get<number>("mainMonitorScreenDpi");
    }
}
