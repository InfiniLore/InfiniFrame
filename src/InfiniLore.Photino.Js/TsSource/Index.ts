// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {sendMessageToHost} from "./MessagingToHost";
import {releasePointerCapture, setPointerCapture} from "./Pointers";
import {getTitleObserver, TitleObserverTarget} from "./Observers";
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
document.addEventListener("keydown", async (e: KeyboardEvent) => {
    if (e.key === "F11") {
        if (document.fullscreenElement) {
            await document.exitFullscreen();
        } else {
            await document.body.requestFullscreen();
        }
    }
});

window.setPointerCapture = setPointerCapture;
window.releasePointerCapture = releasePointerCapture;

getTitleObserver().observe(TitleObserverTarget, {childList: true});
    
    
    
    
    
    
    
    
    
    
    