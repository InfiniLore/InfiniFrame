// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
export function getTitleObserverTarget(): HTMLTitleElement | null {
    return document.querySelector('title');
}

export function getTitleObserver(): MutationObserver {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList") return;
            window.infiniframe.window.features.decorations.setTitle(document.title);
        })
    })
}
