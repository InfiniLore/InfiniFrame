let isToggledToNewTitle = false;
let previousTitle = "";

const fullscreenToggleButton = document.getElementById("fullscreen-toggle-button");
const titleToggleButton = document.getElementById("title-toggle-button");

fullscreenToggleButton?.addEventListener("click", async () => {
    if (!document.fullscreenElement) {
        await document.body.requestFullscreen();
        return;
    }

    if (document.exitFullscreen) {
        await document.exitFullscreen();
    }
});

titleToggleButton?.addEventListener("click", () => {
    if (!isToggledToNewTitle) {
        previousTitle = document.title;
        document.title = "New Title";
        isToggledToNewTitle = true;
        return;
    }

    document.title = previousTitle;
    previousTitle = "";
    isToggledToNewTitle = false;
});
