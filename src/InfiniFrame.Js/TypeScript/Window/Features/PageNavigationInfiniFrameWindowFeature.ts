// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {PageNavigationInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class PageNavigationInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor() {
        super("pageNavigation");
    }

    loadUri(uri: string) {
        return this.post("loadUri", {uri});
    }

    loadPath(path: string) {
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

    getCurrentUrlAsync() {
        return this.get<string | null>("getCurrentUrl");
    }

    getCurrentUriAsync() {
        return this.get<string | null>("getCurrentUri");
    }
}
