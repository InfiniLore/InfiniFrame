// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Internal/Interop/Types/InfiniFrameWindow.h"
#include "Runtime/Internal/Window/CommonWindowState.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
InfiniFrameDialog* InfiniFrameWindow::GetDialog() const {
    return GetCommonWindowState(this)->_dialog.get();
}

// -----------------------------------------------------------------------------------------------------------------
// Callback Setters
// -----------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::SetClosingCallback(const ClosingCallback callback) {
    GetCommonWindowState(this)->_closingCallback = callback;
}

void InfiniFrameWindow::SetClosedCallback(const ClosedCallback callback) {
    GetCommonWindowState(this)->_closedCallback = callback;
}

void InfiniFrameWindow::SetFocusInCallback(const FocusInCallback callback) {
    GetCommonWindowState(this)->_focusInCallback = callback;
}

void InfiniFrameWindow::SetFocusOutCallback(const FocusOutCallback callback) {
    GetCommonWindowState(this)->_focusOutCallback = callback;
}

void InfiniFrameWindow::SetMovedCallback(const MovedCallback callback) {
    GetCommonWindowState(this)->_movedCallback = callback;
}

void InfiniFrameWindow::SetResizedCallback(const ResizedCallback callback) {
    GetCommonWindowState(this)->_resizedCallback = callback;
}

void InfiniFrameWindow::SetMaximizedCallback(const MaximizedCallback callback) {
    GetCommonWindowState(this)->_maximizedCallback = callback;
}

void InfiniFrameWindow::SetRestoredCallback(const RestoredCallback callback) {
    GetCommonWindowState(this)->_restoredCallback = callback;
}

void InfiniFrameWindow::SetMinimizedCallback(const MinimizedCallback callback) {
    GetCommonWindowState(this)->_minimizedCallback = callback;
}

void InfiniFrameWindow::SetDebugEventCallback(const DebugEventCallback callback) {
    GetCommonWindowState(this)->_debugEventCallback = callback;
}

void InfiniFrameWindow::SetFileDroppedCallback(const FileDroppedCallback callback) {
    GetCommonWindowState(this)->_fileDroppedCallback = callback;
}

void InfiniFrameWindow::SetDragDropEnabled(const bool enabled) {
    GetCommonWindowState(this)->_dragDropEnabled = enabled;
}

// -----------------------------------------------------------------------------------------------------------------
// Callback Invokers
// -----------------------------------------------------------------------------------------------------------------
bool InfiniFrameWindow::InvokeClose() const noexcept {
    if (GetCommonWindowState(this)->_closingCallback)
        return GetCommonWindowState(this)->_closingCallback();
    return false;
}

void InfiniFrameWindow::InvokeClosed() const noexcept {
    if (GetCommonWindowState(this)->_closedCallback)
        GetCommonWindowState(this)->_closedCallback();
}

void InfiniFrameWindow::InvokeFocusIn() const noexcept {
    if (GetCommonWindowState(this)->_focusInCallback)
        GetCommonWindowState(this)->_focusInCallback();
}

void InfiniFrameWindow::InvokeFocusOut() const noexcept {
    if (GetCommonWindowState(this)->_focusOutCallback)
        GetCommonWindowState(this)->_focusOutCallback();
}

void InfiniFrameWindow::InvokeMove(const int x, const int y) const noexcept {
    if (GetCommonWindowState(this)->_movedCallback)
        GetCommonWindowState(this)->_movedCallback(x, y);
}

void InfiniFrameWindow::InvokeResize(const int width, const int height) const noexcept {
    if (GetCommonWindowState(this)->_resizedCallback)
        GetCommonWindowState(this)->_resizedCallback(width, height);
}

void InfiniFrameWindow::InvokeMaximized() const noexcept {
    if (GetCommonWindowState(this)->_maximizedCallback)
        GetCommonWindowState(this)->_maximizedCallback();
}

void InfiniFrameWindow::InvokeRestored() const noexcept {
    if (GetCommonWindowState(this)->_restoredCallback)
        GetCommonWindowState(this)->_restoredCallback();
}

void InfiniFrameWindow::InvokeMinimized() const noexcept {
    if (GetCommonWindowState(this)->_minimizedCallback)
        GetCommonWindowState(this)->_minimizedCallback();
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
    if (GetCommonWindowState(this)->_debugEventCallback)
        GetCommonWindowState(this)->_debugEventCallback(
            kind,
            message,
            level,
            uri,
            statusCode,
            timestampUnixMillisecondsUtc,
            platformPayload
            );
}

void InfiniFrameWindow::InvokeFileDropped(
    const char** paths,
    const int count,
    const int x,
    const int y) const noexcept {
    if (GetCommonWindowState(this)->_fileDroppedCallback)
        GetCommonWindowState(this)->_fileDroppedCallback(paths, count, x, y);
}
