// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Window/InfiniFrameWindowImpl.h"
#include "Runtime/Shared/Utilities/StringCopy.h"
// ---------------------------------------------------------------------------------------------------------------------
// Pure property getters that read from InfiniFrameWindowImpl fields with no
// platform-specific logic. Shared across all platforms.
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::GetGrantBrowserPermissions(bool* grant) const {
    *grant = ImplBase()->_grantBrowserPermissions;
}

const char* InfiniFrameWindow::GetUserAgent() const {
#ifdef _WIN32
    return AllocateUtf8FromWide(ImplBase()->_userAgent);
#else
    return AllocateStringCopy(ImplBase()->_userAgent);
#endif
}

void InfiniFrameWindow::GetMediaAutoplayEnabled(bool* enabled) const {
    *enabled = ImplBase()->_mediaAutoplayEnabled;
}

void InfiniFrameWindow::GetFileSystemAccessEnabled(bool* enabled) const {
    *enabled = ImplBase()->_fileSystemAccessEnabled;
}

void InfiniFrameWindow::GetWebSecurityEnabled(bool* enabled) const {
    *enabled = ImplBase()->_webSecurityEnabled;
}

void InfiniFrameWindow::GetJavascriptClipboardAccessEnabled(bool* enabled) const {
    *enabled = ImplBase()->_javascriptClipboardAccessEnabled;
}

void InfiniFrameWindow::GetMediaStreamEnabled(bool* enabled) const {
    *enabled = ImplBase()->_mediaStreamEnabled;
}

void InfiniFrameWindow::GetSmoothScrollingEnabled(bool* enabled) const {
    *enabled = ImplBase()->_smoothScrollingEnabled;
}

void InfiniFrameWindow::GetIgnoreCertificateErrorsEnabled(bool* enabled) const {
    *enabled = ImplBase()->_ignoreCertificateErrorsEnabled;
}

NavigationStartingCallback InfiniFrameWindow::GetNavigationStartingCallback() const {
    return ImplBase()->_navigationStartingCallback;
}

const char* InfiniFrameWindow::GetIconFileName() const {
#ifdef _WIN32
    return AllocateUtf8FromWide(ImplBase()->_iconFileName);
#else
    return AllocateStringCopy(ImplBase()->_iconFileName);
#endif
}

void InfiniFrameWindow::GetBackgroundColor(uint8_t* r, uint8_t* g, uint8_t* b, uint8_t* a) const {
    *r = ImplBase()->_backgroundColorR;
    *g = ImplBase()->_backgroundColorG;
    *b = ImplBase()->_backgroundColorB;
    *a = ImplBase()->_backgroundColorA;
}
