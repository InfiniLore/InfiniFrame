// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Internal/Interop/InfiniFrameWindow.h"
#include "Runtime/Internal/Window/CommonWindowState.h"
#include "Runtime/Internal/Utilities/StringCopy.h"
// ---------------------------------------------------------------------------------------------------------------------
// Pure property getters that read from internal common state with no
// platform-specific logic. Shared across all platforms.
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetGrantBrowserPermissions(bool* grant) const {
    *grant = GetCommonWindowState(this)->_grantBrowserPermissions;
}

const char* InfiniFrameWindow::GetUserAgent() const {
#ifdef _WIN32
    return AllocateUtf8FromWide(GetCommonWindowState(this)->_userAgent);
#else
    return AllocateStringCopy(GetCommonWindowState(this)->_userAgent);
#endif
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_mediaAutoplayEnabled;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_mediaStreamEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_smoothScrollingEnabled;
}

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_ignoreCertificateErrorsEnabled;
}

void InfiniFrameWindow::GetBrowserShortcutsEnabled(bool* enabled) const {
    *enabled = GetCommonWindowState(this)->_browserShortcutsEnabled;
}

NavigationStartingCallback InfiniFrameWindow::GetNavigationStartingCallback() const {
    return GetCommonWindowState(this)->_navigationStartingCallback;
}

const char* InfiniFrameWindow::GetIconFileName() const {
#ifdef _WIN32
    return AllocateUtf8FromWide(GetCommonWindowState(this)->_iconFileName);
#else
    return AllocateStringCopy(GetCommonWindowState(this)->_iconFileName);
#endif
}

void InfiniFrameWindow::GetBackgroundColor(uint8_t* r, uint8_t* g, uint8_t* b, uint8_t* a) const {
    *r = GetCommonWindowState(this)->_backgroundColorR;
    *g = GetCommonWindowState(this)->_backgroundColorG;
    *b = GetCommonWindowState(this)->_backgroundColorB;
    *a = GetCommonWindowState(this)->_backgroundColorA;
}
