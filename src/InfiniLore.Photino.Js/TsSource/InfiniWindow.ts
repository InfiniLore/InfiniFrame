// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
import {IInfiniWindow} from "./Contracts/IInfiniWindow";
import {getTitleObserver, TitleObserverTarget} from "./Observers";
import {HostMessageIds, sendMessageToHost} from "./MessagingToHost";
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export class InfiniWindow implements IInfiniWindow {
    
    constructor() {
        this.assignEventListeners();
        this.assignObservers();
    }
    
    setPointerCapture(element:Element, pointerId: number) {
        element.setPointerCapture(pointerId);
    }

    releasePointerCapture(element:Element, pointerId:number) {
        element.releasePointerCapture(pointerId);
    }
    
    private assignObservers() {
        if (TitleObserverTarget) getTitleObserver().observe(TitleObserverTarget, {childList: true});
    }
    
    private assignEventListeners() {
        document.addEventListener("fullscreenchange", (_: Event) => {
            if (document.fullscreenElement) sendMessageToHost(HostMessageIds.fullscreenEnter);
            else sendMessageToHost(HostMessageIds.fullscreenExit);
        });

        document.addEventListener("keydown", async (e: KeyboardEvent) => {
            if (e.key !== "F11") return;
            if (document.fullscreenElement) await document.exitFullscreen();
            else await document.body.requestFullscreen();
        });
    }
}