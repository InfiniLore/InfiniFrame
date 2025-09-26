// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {sendMessageToHost} from "./MessagingToHost";
import {releasePointerCapture, setPointerCapture} from "./Pointers";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export {};
document.addEventListener("fullscreenchange", (_: Event) => {
    if (document.fullscreenElement) {
        sendMessageToHost("fullscreen:enter");
    } else {
        sendMessageToHost("fullscreen:exit");
    }
});

// TODO add this is a optional?
document.addEventListener("keydown", (e: KeyboardEvent) => {
    if (e.key === "F11") {
        if (document.fullscreenElement) {
            document.exitFullscreen().then();
        } else {
            document.body.requestFullscreen().then();
        }
    }
});

window.setPointerCapture = setPointerCapture;
window.releasePointerCapture = releasePointerCapture;
