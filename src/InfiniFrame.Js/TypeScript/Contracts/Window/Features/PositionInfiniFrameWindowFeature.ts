// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {Point} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface PositionInfiniFrameWindowFeature {
    getLocationAsync(): Promise<Point>;

    getTopAsync(): Promise<number>;

    getLeftAsync(): Promise<number>;

    setLocation(left: number, top: number): void;

    setLeft(left: number): void;

    setTop(top: number): void;

    offset(left: number, top: number): void;

    center(): void;

    centerOnCurrentMonitor(): void;

    centerOnMonitor(monitorIndex: number): void;

    moveWithinCurrentMonitorArea(left: number, top: number): void;
}
