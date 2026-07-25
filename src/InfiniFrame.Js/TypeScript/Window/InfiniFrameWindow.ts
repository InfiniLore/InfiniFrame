// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindow as InfiniFrameWindowContract} from "../Contracts";
import {InfiniFrameWindowFeatures} from "./InfiniFrameWindowFeatures";

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindow implements InfiniFrameWindowContract {
    readonly features = new InfiniFrameWindowFeatures();
}
