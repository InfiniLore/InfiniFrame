// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {DecorationsInfiniFrameWindowFeature as Contract} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class DecorationsInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("decorations");}

    isChromelessAsync() {
        return this.get<boolean>("isChromeless");
    }

    isTransparentAsync() {
        return this.get<boolean>("isTransparent");
    }

    getTitleAsync() {
        return this.get<string | null>("title");
    }

    getIconFilePathAsync() {
        return this.get<string | null>("iconFilePath");
    }

    getLimitLinuxWindowTitleLengthAsync() {
        return this.get<boolean>("limitLinuxWindowTitleLength");
    }

    setTransparent(enabled = true) {
        return this.post("setTransparent", {enabled});
    }

    setTitle(title: string | null) {
        return this.post("setTitle", {title});
    }

    setIconFile(iconFilePath: string) {
        return this.post("setIconFile", {iconFilePath});
    }

    setLimitLinuxWindowTitleLength(enabled = true) {
        return this.post("setLimitLinuxWindowTitleLength", {enabled});
    }
}
