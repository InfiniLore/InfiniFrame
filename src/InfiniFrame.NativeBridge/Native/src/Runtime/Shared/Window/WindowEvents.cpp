// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameDialog* InfiniFrameWindow::GetDialog() const {
    return ImplBase()->_dialog.get();
}

// -----------------------------------------------------------------------------------------------------------------
// Callback Setters
// -----------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback) {
    ImplBase()->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback) {
    ImplBase()->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback) {
    ImplBase()->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback) {
    ImplBase()->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback) {
    ImplBase()->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback) {
    ImplBase()->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback) {
    ImplBase()->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback) {
    ImplBase()->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback) {
    ImplBase()->_minimizedCallback = callback;
}

void InfiniFrameWindow::SetDebugEventCallback(const DebugEventCallback callback) {
    ImplBase()->_debugEventCallback = callback;
}

void InfiniFrameWindow::SetFileDroppedCallback(const FileDroppedCallback callback) {
    ImplBase()->_fileDroppedCallback = callback;
}

void InfiniFrameWindow::SetDragDropEnabled(const bool enabled) {
    ImplBase()->_dragDropEnabled = enabled;
}

// -----------------------------------------------------------------------------------------------------------------
// Callback Invokers
// -----------------------------------------------------------------------------------------------------------------
bool InfiniFrameWindow::InvokeClose() const noexcept {
    if (ImplBase()->_closingCallback)
        return ImplBase()->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept {
    if (ImplBase()->_closedCallback)
        ImplBase()->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept {
    if (ImplBase()->_focusInCallback)
        ImplBase()->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept {
    if (ImplBase()->_focusOutCallback)
        ImplBase()->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(int x, int y) const noexcept {
    if (ImplBase()->_movedCallback)
        ImplBase()->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(int width, int height) const noexcept {
    if (ImplBase()->_resizedCallback)
        ImplBase()->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept {
    if (ImplBase()->_maximizedCallback)
        ImplBase()->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept {
    if (ImplBase()->_restoredCallback)
        ImplBase()->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept {
    if (ImplBase()->_minimizedCallback)
        ImplBase()->_minimizedCallback();
}

void InfiniFrameWindow::InvokeDebugEvent(
    const char* kind,
    const char* message,
    const char* level,
    const char* uri,
    const int statusCode,
    const int64_t timestampUnixMillisecondsUtc,
    const char* platformPayload
) const noexcept {
    if (ImplBase()->_debugEventCallback)
        ImplBase()->_debugEventCallback(
            kind,
            message,
            level,
            uri,
            statusCode,
            timestampUnixMillisecondsUtc,
            platformPayload
        );
}

void InfiniFrameWindow::InvokeFileDropped(const char** paths, const int count, const int x, const int y) const noexcept {
    if (ImplBase()->_fileDroppedCallback)
        ImplBase()->_fileDroppedCallback(paths, count, x, y);
}
