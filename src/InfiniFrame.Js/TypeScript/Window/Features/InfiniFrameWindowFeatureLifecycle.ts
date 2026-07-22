// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindowFeatureLifecycle as Contract,WindowLifecycleState} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeatureLifecycle extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("lifecycle");}

    getStateAsync() {
        return this.get<WindowLifecycleState>("state");
    }

    isClosedOrClosingAsync() {
        return this.get<boolean>("isClosedOrClosing");
    }

    close() {
        return this.post("close");
    }
}
