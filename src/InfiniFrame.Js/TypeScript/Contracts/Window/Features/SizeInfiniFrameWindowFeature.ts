// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import type {ResizeOrigin, Size} from "./WindowFeatureTypes";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export interface SizeInfiniFrameWindowFeature {
    getSizeAsync(): Promise<Size>;

    getHeightAsync(): Promise<number>;

    getWidthAsync(): Promise<number>;

    getMaxSizeAsync(): Promise<Size>;

    getMaxHeightAsync(): Promise<number>;

    getMaxWidthAsync(): Promise<number>;

    getMinSizeAsync(): Promise<Size>;

    getMinHeightAsync(): Promise<number>;

    getMinWidthAsync(): Promise<number>;

    isResizableAsync(): Promise<boolean>;

    setSize(width: number, height: number): void;

    setHeight(height: number): void;

    setWidth(width: number): void;

    setMaxSize(width: number, height: number): void;

    setMaxHeight(height: number): void;

    setMaxWidth(width: number): void;

    setMinSize(width: number, height: number): void;

    setMinHeight(height: number): void;

    setMinWidth(width: number): void;

    resize(widthOffset: number, heightOffset: number, origin: ResizeOrigin): void;

    setResizable(resizable?: boolean): void;
}
