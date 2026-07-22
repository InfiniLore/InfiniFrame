// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {InfiniFrameWindowFeatureSize as Contract,ResizeOrigin,Size} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class SizeInfiniFrameWindowFeature extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("size");}

    getSizeAsync() {
        return this.get<Size>("size");
    }

    getHeightAsync() {
        return this.get<number>("height");
    }

    getWidthAsync() {
        return this.get<number>("width");
    }

    getMaxSizeAsync() {
        return this.get<Size>("maxSize");
    }

    getMaxHeightAsync() {
        return this.get<number>("maxHeight");
    }

    getMaxWidthAsync() {
        return this.get<number>("maxWidth");
    }

    getMinSizeAsync() {
        return this.get<Size>("minSize");
    }

    getMinHeightAsync() {
        return this.get<number>("minHeight");
    }

    getMinWidthAsync() {
        return this.get<number>("minWidth");
    }

    isResizableAsync() {
        return this.get<boolean>("isResizable");
    }

    setSize(width: number, height: number) {
        return this.post("setSize", {width, height});
    }

    setHeight(height: number) {
        return this.post("setHeight", {height});
    }

    setWidth(width: number) {
        return this.post("setWidth", {width});
    }

    setMaxSize(width: number, height: number) {
        return this.post("setMaxSize", {width, height});
    }

    setMaxHeight(height: number) {
        return this.post("setMaxHeight", {height});
    }

    setMaxWidth(width: number) {
        return this.post("setMaxWidth", {width});
    }

    setMinSize(width: number, height: number) {
        return this.post("setMinSize", {width, height});
    }

    setMinHeight(height: number) {
        return this.post("setMinHeight", {height});
    }

    setMinWidth(width: number) {
        return this.post("setMinWidth", {width});
    }

    resize(widthOffset: number, heightOffset: number, origin: ResizeOrigin) {
        return this.post("resize", {widthOffset, heightOffset, origin});
    }

    setResizable(resizable = true) {
        return this.post("setResizable", {resizable});
    }
}
