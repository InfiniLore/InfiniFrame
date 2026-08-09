// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {JavaScriptInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class JavaScriptInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("javaScript");}

    evalAsync(script: string): Promise<string | null> {
        return this.get<string | null>("eval", {script});
    }
}
