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

window.setPointerCapture = setPointerCapture;
window.releasePointerCapture = releasePointerCapture;
