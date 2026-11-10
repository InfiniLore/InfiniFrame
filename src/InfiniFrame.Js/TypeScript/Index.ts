/**
 * Entry point for the InfiniFrame JavaScript bundle. Assembles and exports the `window.infiniframe` namespace.
 * @module Index
 */

// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import InfiniFrame from "./InfiniFrame";
import { initWindowExternalBridge } from "./Interop/NativeInterop/blazorExternalBridge";
import { initBlazorModulesFetchPatch } from "./Interop/NativeInterop/blazorFetchPatch";
import {
	initBlazorCustomElementsPatch,
	initCustomElements,
} from "./Interop/NativeInterop/customElements";
import { installNativeInteropBridge } from "./Interop/NativeInterop/NativeInteropBridge";
import { getSetupGuard } from "./Interop/NativeInterop/setupGuard";
import windowChrome from "./Window/WindowChrome";

const setup = getSetupGuard();
installNativeInteropBridge(setup);
initWindowExternalBridge(setup);
initBlazorModulesFetchPatch(setup);
initBlazorCustomElementsPatch(setup);
initCustomElements(setup);

if (
	!window.infiniframe?.messaging ||
	!window.infiniframe?.window?.features ||
	!window.infiniframe?.utils
) {
	window.infiniframe = new InfiniFrame(window.infiniframe);
}

window.infiniframe.windowChrome = windowChrome;

console.log("InfiniFrame WebView JavaScript bridge initialized.");
