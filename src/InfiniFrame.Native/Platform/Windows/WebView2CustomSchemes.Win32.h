#pragma once
/**
 * @file WebView2CustomSchemes.Win32.h
 * @brief WebView2 custom scheme registration helpers.
 */

#ifndef INFINIFRAME_PLATFORM_WINDOWS_WEBVIEW2CUSTOMSCHEMES_WIN32_H
#define INFINIFRAME_PLATFORM_WINDOWS_WEBVIEW2CUSTOMSCHEMES_WIN32_H

#include <string>
#include <vector>

#include <WebView2.h>

namespace InfiniFrame::Platform::Windows {
    bool TryRegisterCustomSchemes(
        ICoreWebView2EnvironmentOptions* options,
        const std::vector<std::wstring>& customSchemeNames
        );
}

#endif // INFINIFRAME_PLATFORM_WINDOWS_WEBVIEW2CUSTOMSCHEMES_WIN32_H
