#pragma once
/**
 * @file InfiniFrameWindowImpl.h
 * @brief Shared state for all platform InfiniFrameWindow::Impl structs.
 *
 * This is an INTERNAL header — included only by platform Window.cpp/.mm files,
 * never by consumers of InfiniFrame. It defines the fields that are identical
 * across Windows, Linux, and macOS implementations.
 *
 * Each platform defines:
 *
 *   struct InfiniFrameWindow::Impl : InfiniFrameWindowImpl { ... platform handles ... };
 */

#ifndef INFINIFRAME_CORE_WINDOWIMPL_H
#define INFINIFRAME_CORE_WINDOWIMPL_H

#include "../Types/Basic.h"
#include "../Types/Callbacks.h"
#include "InfiniFrameDialog.h"

#include <memory>
#include <string>
#include <vector>

class InfiniFrameWindow;

struct InfiniFrameWindowImpl
{
    // -----------------------------------------------------------------------------------------------------------------=
    // Callbacks
    // -----------------------------------------------------------------------------------------------------------------=

    WebMessageReceivedCallback _webMessageReceivedCallback = nullptr;
    WebResourceRequestedCallback _customSchemeCallback = nullptr;
    ResizedCallback _resizedCallback = nullptr;
    MaximizedCallback _maximizedCallback = nullptr;
    RestoredCallback _restoredCallback = nullptr;
    MinimizedCallback _minimizedCallback = nullptr;
    MovedCallback _movedCallback = nullptr;
    ClosingCallback _closingCallback = nullptr;
    FocusInCallback _focusInCallback = nullptr;
    FocusOutCallback _focusOutCallback = nullptr;

    // -----------------------------------------------------------------------------------------------------------------=
    // Feature flags
    // -----------------------------------------------------------------------------------------------------------------=

    bool _transparentEnabled = false;
    bool _contextMenuEnabled = true;
    bool _zoomEnabled = true;
    bool _devToolsEnabled = false;
    bool _grantBrowserPermissions = false;
    bool _mediaAutoplayEnabled = false;
    bool _fileSystemAccessEnabled = false;
    bool _webSecurityEnabled = true;
    bool _javascriptClipboardAccessEnabled = false;
    bool _mediaStreamEnabled = false;
    bool _smoothScrollingEnabled = true;
    bool _ignoreCertificateErrorsEnabled = false;

    // -----------------------------------------------------------------------------------------------------------------=
    // String state  (NativeString = std::wstring on Windows, std::string elsewhere)
    // -----------------------------------------------------------------------------------------------------------------=

    NativeString _windowTitle;
    NativeString _startUrl;
    NativeString _startString;
    NativeString _userAgent;
    NativeString _browserControlInitParameters;
    NativeString _iconFileName;

    std::vector<NativeString> _customSchemeNames;

    // -----------------------------------------------------------------------------------------------------------------=
    // Ownership
    // -----------------------------------------------------------------------------------------------------------------=

    InfiniFrameWindow *_parent = nullptr;
    std::unique_ptr<InfiniFrameDialog> _dialog;
};

#endif // INFINIFRAME_CORE_WINDOWIMPL_H