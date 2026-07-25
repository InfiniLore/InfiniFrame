// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {PositionInfiniFrameWindowFeature as Contract,Point} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class PositionInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("position");}

    getLocationAsync() {
        return this.get<Point>("location");
    }

    getTopAsync() {
        return this.get<number>("top");
    }

    getLeftAsync() {
        return this.get<number>("left");
    }

    setLocation(left: number, top: number) {
        return this.post("setLocation", {left, top});
    }

    setLeft(left: number) {
        return this.post("setLeft", {left});
    }

    setTop(top: number) {
        return this.post("setTop", {top});
    }

    offset(left:number, top:number) {
        return this.post("offset", {left, top});
    }

    center() {
        return this.post("center");
    }

    centerOnCurrentMonitor() {
        return this.post("centerOnCurrentMonitor");
    }

    centerOnMonitor(monitorIndex: number) {
        return this.post("centerOnMonitor", {monitorIndex});
    }

    moveWithinCurrentMonitorArea(left: number, top: number) {
        return this.post("moveWithinCurrentMonitorArea", {left, top});
    }
}
