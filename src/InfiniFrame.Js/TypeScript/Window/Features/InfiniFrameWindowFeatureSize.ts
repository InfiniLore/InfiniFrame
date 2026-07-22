import type {InfiniFrameWindowFeatureSize as Contract,ResizeOrigin,Size} from "../../Contracts";
import {InfiniFrameWindowFeature} from "../InfiniFrameWindowFeature";
export class InfiniFrameWindowFeatureSize extends InfiniFrameWindowFeature implements Contract {
    constructor(){super("size");}
    getSizeAsync=()=>this.get<Size>("size"); getHeightAsync=()=>this.get<number>("height"); getWidthAsync=()=>this.get<number>("width");
    getMaxSizeAsync=()=>this.get<Size>("maxSize"); getMaxHeightAsync=()=>this.get<number>("maxHeight"); getMaxWidthAsync=()=>this.get<number>("maxWidth");
    getMinSizeAsync=()=>this.get<Size>("minSize"); getMinHeightAsync=()=>this.get<number>("minHeight"); getMinWidthAsync=()=>this.get<number>("minWidth");
    isResizableAsync=()=>this.get<boolean>("isResizable");
    setSize=(width:number,height:number)=>this.post("setSize",{width,height}); setHeight=(height:number)=>this.post("setHeight",{height}); setWidth=(width:number)=>this.post("setWidth",{width});
    setMaxSize=(width:number,height:number)=>this.post("setMaxSize",{width,height}); setMaxHeight=(height:number)=>this.post("setMaxHeight",{height}); setMaxWidth=(width:number)=>this.post("setMaxWidth",{width});
    setMinSize=(width:number,height:number)=>this.post("setMinSize",{width,height}); setMinHeight=(height:number)=>this.post("setMinHeight",{height}); setMinWidth=(width:number)=>this.post("setMinWidth",{width});
    resize=(widthOffset:number,heightOffset:number,origin:ResizeOrigin)=>this.post("resize",{widthOffset,heightOffset,origin});
    setResizable=(resizable=true)=>this.post("setResizable",{resizable});
}
