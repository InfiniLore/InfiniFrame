export async function toggleFullscreen() {
    if (!document.fullscreenElement) {
        await document.body.requestFullscreen();
    }
    else if (document.exitFullscreen) {
        await document.exitFullscreen();
    }
}

export function isFullscreenActive() {
    return document.fullscreenElement !== null;
}

export function getDocumentTitle() {
    return document.title;
}

export function setDocumentTitle(title) {
    document.title = title;
}
