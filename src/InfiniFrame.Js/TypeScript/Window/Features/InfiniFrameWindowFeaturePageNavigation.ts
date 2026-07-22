// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindowFeaturePageNavigation as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniFrameWindowFeaturePageNavigation extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("pageNavigation");}

    loadUri(uri: string) {
        return this.post("loadUri", {uri});
    }

    loadPath(path:string) {
        return this.post("loadPath", {path});
    }

    tryLoadUriAsync(uri: string) {
        return this.get<boolean>("tryLoadUri", {uri});
    }

    tryLoadPathAsync(path: string) {
        return this.get<boolean>("tryLoadPath", {path});
    }

    loadRawString(content: string) {
        return this.post("loadRawString", {content});
    }
}
